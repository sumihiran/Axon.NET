namespace Axon;

using Axon.Messaging;

/// <summary>
/// Interface for a component that processes Messages.
/// </summary>
/// <typeparam name="TMessage">The message type this handler can process.</typeparam>
public interface IMessageHandler<in TMessage>
    where TMessage : IMessage<object>
{
    /// <summary>
    /// Handle the given message.
    /// </summary>
    /// <param name="message">The message to be handled.</param>
    /// <returns>The result of the message handling.</returns>
    Task<object?> HandleAsync(TMessage message);

    /// <summary>
    /// Indicates whether this handler can handle the given message.
    /// </summary>
    /// <param name="message">The message to verify.</param>
    /// <returns><c>true</c> if this handler can handle the message, otherwise <c>false</c>.</returns>
    public bool CanHandle(object message) => true;
}
