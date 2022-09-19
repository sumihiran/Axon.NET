namespace Axon;

using Axon.Messaging;

/// <summary>
/// QueryBus extension methods.
/// </summary>
public static class QueryBusExtensions
{
    /// <summary>
    /// Subscribe the given <paramref name="handler"/> to queries with the given <paramref name="queryName"/> and
    /// <paramref name="responseType"/>. Multiple handlers may subscribe to the same combination of queryName and
    /// responseType.
    /// </summary>
    /// <param name="queryBus">The query bus.</param>
    /// <param name="queryName">The name of the query to subscribe.</param>
    /// <param name="responseType">The type of response the subscribed component answers with.</param>
    /// <param name="handler">A handler that handles the query.</param>
    /// <typeparam name="TPayload">The type of payload.</typeparam>
    /// <typeparam name="TResponse">The type of response expected from this query.</typeparam>
    /// <returns>A <see cref="Task"/> containing a handle to un-subscribe the query handler.</returns>
    public static Task<IAsyncDisposable> SubscribeAsync<TPayload, TResponse>(
        this IQueryBus queryBus,
        string queryName,
        Type responseType,
        MessageHandler<IQueryMessage<TPayload, TResponse>> handler)
        where TPayload : class
        =>
            queryBus.SubscribeAsync<IQueryMessage<TPayload, TResponse>, TResponse>(queryName, responseType, handler);

    /// <summary>
    /// Subscribe the given <paramref name="handler"/> to queries with the given <paramref name="queryName"/>. Multiple
    /// handlers may subscribe to the same combination of queryName and responseType.
    /// </summary>
    /// <param name="queryBus">The query bus.</param>
    /// <param name="queryName">The name of the query to subscribe.</param>
    /// <param name="handler">A handler that handles the query.</param>
    /// <typeparam name="TPayload">The type of payload.</typeparam>
    /// <typeparam name="TResponse">The type of response expected from this query.</typeparam>
    /// <returns>A <see cref="Task"/> containing a handle to un-subscribe the query handler.</returns>
    public static Task<IAsyncDisposable> SubscribeAsync<TPayload, TResponse>(
        this IQueryBus queryBus,
        string queryName,
        MessageHandler<IQueryMessage<TPayload, TResponse>> handler)
        where TPayload : class
        =>
            queryBus.SubscribeAsync<IQueryMessage<TPayload, TResponse>, TResponse>(
                queryName,
                typeof(TResponse),
                handler);
}
