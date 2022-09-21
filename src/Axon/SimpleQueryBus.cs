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
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, QuerySubscription>> subscriptions = new();

    /// <summary>
    /// Gets the subscriptions for this query bus. While the returned dictionary is unmodifiable, it may or may not
    /// reflect changes made to the subscriptions after the call was made.
    /// </summary>
    public ImmutableDictionary<string, ImmutableList<QuerySubscription>> Subscriptions => this.GetSubscriptions();

    /// <inheritdoc />
    public Task<IAsyncDisposable> SubscribeAsync<TMessage, TResponse>(
        string queryName,
        Type responseType,
        MessageHandler<TMessage> handler)
        where TMessage : IQueryMessage<object, TResponse>
    {
        var querySubscription = new QuerySubscription(responseType, handler);
        _ = this.subscriptions.AddOrUpdate(
            queryName,
            _ =>
            {
                var handlers = new ConcurrentDictionary<int, QuerySubscription>();
                handlers.TryAdd(querySubscription.GetHashCode(), querySubscription);
                return handlers;
            },
            (_, handlers) =>
            {
                handlers.AddOrUpdate(
                    querySubscription.GetHashCode(),
                    _ => querySubscription,
                    (_, subscription) => subscription);
                return handlers;
            });

        return Task.FromResult(
            (IAsyncDisposable)new Registration(() => this.Unsubscribe(queryName, querySubscription)));
    }

    /// <inheritdoc />
    public async Task<TResponse?> QueryAsync<TResponse>(IQueryMessage<object, TResponse> query)
    {
        var handlers = this.GetHandlersForMessage(query);
        if (handlers.IsEmpty)
        {
            throw new NoHandlerForQueryException(
                $"No handler found for {query.QueryName} with response type {query.ResponseType}");
        }

        var handler = handlers.First();

        return ConvertResponse(await handler.HandleAsync(query).ConfigureAwait(false), query.ResponseType);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<TResponse?> ScatterGatherAsync<TResponse>(IQueryMessage<object, TResponse> query)
    {
        var handlers = this.GetHandlersForMessage(query);
        if (handlers.IsEmpty)
        {
            yield break;
        }

        foreach (var handler in handlers)
        {
            yield return ConvertResponse(await handler.HandleAsync(query).ConfigureAwait(false), query.ResponseType);
        }
    }

    private static TResponse? ConvertResponse<TResponse>(object? response, IResponseType<TResponse> responseType)
        => responseType.Convert(response);

    private void Unsubscribe(string queryName, QuerySubscription querySubscription)
    {
        if (this.subscriptions.TryGetValue(queryName, out var querySubscriptions))
        {
            querySubscriptions.TryRemove(
                new KeyValuePair<int, QuerySubscription>(querySubscription.GetHashCode(), querySubscription));
        }
    }

    private ImmutableList<IMessageHandler> GetHandlersForMessage<TResponse>(
        IQueryMessage<object, TResponse> queryMessage)
    {
        var responseType = queryMessage.ResponseType;
        if (this.subscriptions.TryGetValue(queryMessage.QueryName, out var querySubscriptions))
        {
            return querySubscriptions.Values
                .Where(querySubscription => responseType.Matches(querySubscription.ResponseType))
                .Select(querySubscription => querySubscription.QueryHandler)
                .ToImmutableList();
        }

        return ImmutableList<IMessageHandler>.Empty;
    }

    private ImmutableDictionary<string, ImmutableList<QuerySubscription>> GetSubscriptions() =>
        this.subscriptions.ToImmutableDictionary(s => s.Key, kv => kv.Value.Values.ToImmutableList());

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
