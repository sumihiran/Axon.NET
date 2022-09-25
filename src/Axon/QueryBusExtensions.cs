namespace Axon;

using Axon.Messaging;

/// <summary>
/// QueryBus extensions.
/// </summary>
public static class QueryBusExtensions
{
    /// <summary>
    /// Subscribe the given <paramref name="handler"/> to queries with the given <paramref name="queryName"/> and
    /// <paramref name="responseType"/>. Multiple handlers may subscribe to the same combination of queryName/responseType.
    /// </summary>
    /// <param name="queryBus">The queryBus.</param>
    /// <param name="queryName">The name of the query to subscribe.</param>
    /// <param name="responseType">The type of response the subscribed component answers with.</param>
    /// <param name="handler">A handler that handles the query.</param>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <returns>A <see cref="Task"/> containing a handle to un-subscribe the query handler.</returns>
    public static Task<IAsyncDisposable> SubscribeAsync<TResponse>(
        this IQueryBus queryBus,
        string queryName,
        Type responseType,
        MessageHandlerCallback<IQueryMessage<object, TResponse>> handler)
        where TResponse : class
    {
        var wrappedHandler = new WrappedMessageHandlerCallback<IQueryMessage<object, TResponse>>(handler);
        return queryBus.SubscribeAsync(queryName, responseType, wrappedHandler);
    }
}
