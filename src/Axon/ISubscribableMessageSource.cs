namespace Axon;

/// <summary>
/// Interface for a source of message to which message processors can subscribe.
/// </summary>
/// <typeparam name="TMessage">the message type.</typeparam>
public interface ISubscribableMessageSource<TMessage>
{
    /// <summary>
    /// Subscribe the given <paramref name="messageProcessor"/> to this message source. When subscribed,
    /// it will receive all messages published to this source.
    ///
    /// If the given <paramref name="messageProcessor"/> is already subscribed, nothing happens.
    /// </summary>
    /// <param name="messageProcessor">The message processor to subscribe.</param>
    /// <returns>
    /// A <see cref="Task"/> representing asynchronous operation which results in a handle to unsubscribe
    /// the <paramref name="messageProcessor"/>. When unsubscribed it will no longer receive messages.
    /// </returns>
    Task<IAsyncDisposable> SubscribeAsync(Func<List<object>, Task> messageProcessor);
}
