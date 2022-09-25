namespace Axon;

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Axon.Messaging;
using Axon.Messaging.ResponseTypes;

/// <summary>
/// Implementation of the <see cref="IQueryBus"/> that dispatches Query messages to their appropriate QueryHandlers.
/// </summary>
public class SimpleQueryBus : IQueryBus
{
    private readonly ConcurrentDictionary<string, HashSet<IQuerySubscription>> subscriptions = new();

    /// <summary>
    /// Gets the subscriptions for this query bus. While the returned dictionary is unmodifiable, it may or may not
    /// reflect changes made to the subscriptions after the call was made.
    /// </summary>
    public ImmutableDictionary<string, ImmutableList<IQuerySubscription>> Subscriptions => this.GetSubscriptions();

    /// <inheritdoc />
    public Task<IAsyncDisposable> SubscribeAsync<TResponse>(
        string queryName,
        Type responseType,
        IMessageHandler<IQueryMessage<object, TResponse>> handler)
        where TResponse : class
    {
        var querySubscription = new QuerySubscription<TResponse>(responseType, handler);
        _ = this.subscriptions.AddOrUpdate(
            queryName,
            _ => new HashSet<IQuerySubscription> { querySubscription },
            (_, handlers) =>
            {
                if (!handlers.Contains(querySubscription))
                {
                    handlers.Add(querySubscription);
                }

                return handlers;
            });

        return Task.FromResult(
            (IAsyncDisposable)new Registration(() => this.Unsubscribe(queryName, querySubscription)));
    }

    /// <inheritdoc />
    public async Task<IQueryResponseMessage<TResponse>> QueryAsync<TResponse>(
        IQueryMessage<object, TResponse> query)
        where TResponse : class
    {
        var handlers = this.GetHandlersForMessage(query);
        if (handlers.IsEmpty)
        {
            throw new NoHandlerForQueryException(
                $"No handler found for {query.QueryName} with response type {query.ResponseType}");
        }

        var handler = handlers.First();

        var result = ConvertResponse(await handler.HandleAsync(query).ConfigureAwait(false), query.ResponseType);

        return GenericQueryResponseMessage.AsNullableResponseMessage<TResponse>(
            query.ResponseType.ResponseMessagePayloadType, result);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IQueryResponseMessage<TResponse>> ScatterGatherAsync<TResponse>(
        IQueryMessage<object, TResponse> query)
        where TResponse : class
    {
        var handlers = this.GetHandlersForMessage(query);
        if (handlers.IsEmpty)
        {
            yield break;
        }

        foreach (var handler in handlers)
        {
            var result = ConvertResponse(await handler.HandleAsync(query).ConfigureAwait(false), query.ResponseType);

            if (result is null)
            {
                continue;
            }

            // TODO: Add default QueryHandler Error callback
            yield return GenericQueryResponseMessage.AsNullableResponseMessage<TResponse>(
                query.ResponseType.ResponseMessagePayloadType, result);
        }
    }

    private static TResponse? ConvertResponse<TResponse>(object? response, IResponseType<TResponse> responseType)
        => responseType.Convert(response);

    private void Unsubscribe(string queryName, IQuerySubscription querySubscription)
    {
        if (this.subscriptions.TryGetValue(queryName, out var querySubscriptions))
        {
            querySubscriptions.Remove(querySubscription);
        }
    }

    private ImmutableList<IMessageHandler<IQueryMessage<object, TResponse>>> GetHandlersForMessage<TResponse>(
        IQueryMessage<object, TResponse> queryMessage)
        where TResponse : class
    {
        var responseType = queryMessage.ResponseType;
        if (this.subscriptions.TryGetValue(queryMessage.QueryName, out var querySubscriptions))
        {
            return querySubscriptions
                .Select(querySubscription => querySubscription as QuerySubscription<TResponse>)
                .Where(querySubscription => querySubscription is not null)
                .Where(querySubscription => responseType.Matches(querySubscription!.ResponseType))
                .Select(querySubscription => querySubscription!.QueryHandler)
                .ToImmutableList();
        }

        return ImmutableList<IMessageHandler<IQueryMessage<object, TResponse>>>.Empty;
    }

    private ImmutableDictionary<string, ImmutableList<IQuerySubscription>> GetSubscriptions()
        => this.subscriptions.ToImmutableDictionary(s => s.Key, kv => kv.Value.Select(q => q).ToImmutableList());

    private class Registration : IAsyncDisposable
    {
        private readonly Action unsubscribeAction;

        public Registration(Action unsubscribeAction) => this.unsubscribeAction = unsubscribeAction;

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            this.unsubscribeAction.Invoke();
            return ValueTask.CompletedTask;
        }
    }
}
