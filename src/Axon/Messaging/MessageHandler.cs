namespace Axon.Messaging;

/// <summary>
/// A component that processes messages.
/// </summary>
/// <typeparam name="TMessage">The type of the Message to be handled.</typeparam>
public abstract class MessageHandler<TMessage> : IMessageHandler
    where TMessage : IMessage<object>
{
    /// <summary>
    /// Handle the given message.
    /// </summary>
    /// <param name="message">The message to be handled.</param>
    /// <returns>The result of the message handling.</returns>
    public abstract Task<object?> HandleAsync(TMessage message);

    /// <inheritdoc />
    public Task<object?> HandleAsync(IMessage<object> message)
        => this.HandleAsync((TMessage)message);

    /// <inheritdoc />
    public virtual bool CanHandle(object message) => message is TMessage;
}
