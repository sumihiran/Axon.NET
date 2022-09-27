namespace Axon;

using Axon.Messaging;

/// <summary>
/// Represents a CommandHandler callback that is wrapped inside a message handler.
/// </summary>
/// <typeparam name="TMessage">The message type this handler can process.</typeparam>
internal class WrappedMessageHandlerCallback<TMessage> : IMessageHandler<TMessage>,
    IEquatable<WrappedMessageHandlerCallback<TMessage>>, IEquatable<MessageHandlerCallback<TMessage>>
    where TMessage : IMessage<object>
{
    private readonly MessageHandlerCallback<TMessage> callback;

    /// <summary>
    /// Initializes a new instance of the <see cref="WrappedMessageHandlerCallback{TMessage}"/> class.
    /// </summary>
    /// <param name="callback">The <see cref="MessageHandlerCallback{TMessage}"/>.</param>
    public WrappedMessageHandlerCallback(MessageHandlerCallback<TMessage> callback) =>
        this.callback = callback;

    /// <inheritdoc />
    public Task<object?> HandleAsync(TMessage message) => this.callback(message);

    /// <inheritdoc />
    public bool CanHandle(object message) => true;

    /// <summary>
    /// Returns the wrapped callback.
    /// </summary>
    /// <returns>The wrapped callback.</returns>
    public MessageHandlerCallback<TMessage> Unwrap() => this.callback;

    /// <inheritdoc />
    public bool Equals(MessageHandlerCallback<TMessage>? other) => this.callback.Equals(other);

    /// <inheritdoc />
    public bool Equals(WrappedMessageHandlerCallback<TMessage>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.callback.Equals(other.callback);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is MessageHandlerCallback<ICommandMessage<object>> handlerCallback)
        {
            return this.Equals(handlerCallback);
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return this.Equals((WrappedMessageHandlerCallback<TMessage>)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode() => this.callback.GetHashCode();
}
