namespace Axon.Messaging;

/// <summary>
/// The message handler callback.
/// </summary>
/// <param name="message">The message to be handled.</param>
/// <typeparam name="TMessage">The message type.</typeparam>
/// <returns>A <see cref="Task"/> containing the result of the message processing.</returns>
public delegate Task<object?> MessageHandlerCallback<in TMessage>(TMessage message)
    where TMessage : IMessage<object>;
