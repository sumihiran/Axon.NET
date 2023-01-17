namespace Axon.Messaging;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// The message handler callback.
/// </summary>
/// <param name="message">The message to be handled.</param>
/// <typeparam name="TMessage">The message type.</typeparam>
/// <returns>A <see cref="Task"/> containing the result of the message processing.</returns>
public delegate Task<object?> MessageHandlerCallback<in TMessage>(TMessage message)
    where TMessage : IMessage<object>;

/// <summary>
/// The message handler extensions.
/// </summary>
[SuppressMessage("ReSharper", "SA1649:FileNameMustMatchTypeName", Justification = "Delegate extensions")]
internal static class MessageHandlerCallback
{
    /// <summary>
    /// Wraps the <see cref="MessageHandlerCallback"/> delegate inside a
    /// <see cref="WrappedMessageHandlerCallback{TMessage}"/> instance.
    /// </summary>
    /// <param name="handlerCallback">The <see cref="MessageHandlerCallback"/> delegate.</param>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>A <see cref="WrappedMessageHandlerCallback{TMessage}"/>.</returns>
    public static IMessageHandler<TMessage> Wrap<TMessage>(this MessageHandlerCallback<TMessage> handlerCallback)
        where TMessage : IMessage<object> => new WrappedMessageHandlerCallback<TMessage>(handlerCallback);
}
