namespace Axon;

using Axon.Messaging;

/// <summary>
/// The mechanism that dispatches Query messages to their appropriate QueryHandlers. QueryHandlers can subscribe and
/// un-subscribe to specific queries (identified by their <see cref="IQueryMessage{TPayload,TResponse}.QueryName"/>
/// and  <see cref="IQueryMessage{TPayload,TResponse}.ResponseType"/> on the query bus.
/// There may be multiple handlers for each combination of queryName/responseType.
/// </summary>
public interface IQueryBus
{
    /// <summary>
    /// Subscribe the given <paramref name="handler"/> to queries with the given <paramref name="queryName"/> and
    /// <paramref name="responseType"/>. Multiple handlers may subscribe to the same combination of queryName/responseType.
    /// </summary>
    /// <param name="queryName">The name of the query to subscribe.</param>
    /// <param name="responseType">The type of response the subscribed component answers with.</param>
    /// <param name="handler">A handler that handles the query.</param>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <returns>A <see cref="Task"/> containing a handle to un-subscribe the query handler.</returns>
    public Task<IAsyncDisposable> SubscribeAsync<TResponse>(
        string queryName,
        Type responseType,
        IMessageHandler<IQueryMessage<object, TResponse>> handler)
        where TResponse : class;

    /// <summary>
    /// Dispatch the given <paramref name="query"/> to a single QueryHandler subscribed to the given
    /// <paramref name="query"/>'s queryName and responseType.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <typeparam name="TResponse">The type of response expected from this query.</typeparam>
    /// <returns>A <see cref="Task{TResult}"/>  that resolves when the response is available.</returns>
    Task<IQueryResponseMessage<TResponse>> QueryAsync<TResponse>(IQueryMessage<object, TResponse> query)
        where TResponse : class;

    /// <summary>
    /// Dispatch the given <paramref name="query"/> to all QueryHandlers subscribed matched with
    /// <paramref name="query"/>'s queryName/responseType. Returns a async enumerable of results which asynchronously
    /// iterate until all handlers have processed the request or when the timeout occurs.
    /// If no handlers are available to provide a result, or when all available handlers throw an exception while
    /// attempting to do so, the returned AsyncEnumerable is empty.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <typeparam name="TResponse">The response type of the query.</typeparam>
    /// <returns>An AsyncEnumerable of the query results.</returns>
    /// TODO: Add deadline
    IAsyncEnumerable<IQueryResponseMessage<TResponse>> ScatterGatherAsync<TResponse>(
        IQueryMessage<object, TResponse> query)
        where TResponse : class;

    // TODO: Query Subscriptions
}
