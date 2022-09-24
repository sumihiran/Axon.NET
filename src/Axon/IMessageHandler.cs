namespace Axon;

using Axon.Messaging;

/// <summary>
/// Interface for a component that processes Messages.
/// </summary>
public interface IMessageHandler
{
    /// <summary>
    /// Handle abstract message.
    /// </summary>
    /// <param name="message">A message.</param>
    /// <returns>A <see cref="Task"/> containing the result of the message processing.</returns>
    /// <seealso cref="MessageHandler{TMessage}"/>
    Task<object?> HandleAsync(IMessage<object> message);

    /// <summary>
    /// Indicates whether this handler can handle the given message.
    /// </summary>
    /// <param name="message">The message to verify.</param>
    /// <returns><c>true</c> if this handler can handle the message, otherwise <c>false</c>.</returns>
    bool CanHandle(object message);
}
