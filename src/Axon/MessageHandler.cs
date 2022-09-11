namespace Axon;

using Axon.Messaging;

/// <summary>
/// A component that processes Messages.
/// </summary>
/// <typeparam name="TMessage">The type of the Message to be handled.</typeparam>
public class MessageHandler<TMessage>
    where TMessage : IMessage<object>
{
    /// <summary>
    /// Handle the given message.
    /// </summary>
    /// <param name="message">The message to be handled.</param>
    /// <returns>The result of the message handling.</returns>
    public virtual Task<object> HandleAsync(TMessage message) => Task.FromResult((object)message);
}
