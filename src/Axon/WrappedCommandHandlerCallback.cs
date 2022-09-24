namespace Axon;

using Axon.Messaging;

/// <summary>
/// Represents a CommandHandler callback that is wrapped inside a message handler.
/// </summary>
internal class WrappedCommandHandlerCallback : MessageHandler<ICommandMessage<object>>,
    IEquatable<WrappedCommandHandlerCallback>, IEquatable<MessageHandlerCallback<ICommandMessage<object>>>
{
    private readonly MessageHandlerCallback<ICommandMessage<object>> callback;

    /// <summary>
    /// Initializes a new instance of the <see cref="WrappedCommandHandlerCallback"/> class.
    /// </summary>
    /// <param name="callback">The <see cref="MessageHandlerCallback{TMessage}"/>.</param>
    public WrappedCommandHandlerCallback(MessageHandlerCallback<ICommandMessage<object>> callback) =>
        this.callback = callback;

    /// <inheritdoc />
    public override Task<object?> HandleAsync(ICommandMessage<object> message) => this.callback(message);

    /// <summary>
    /// Returns the wrapped callback.
    /// </summary>
    /// <returns>The wrapped callback.</returns>
    public MessageHandlerCallback<ICommandMessage<object>> Unwrap() => this.callback;

    /// <inheritdoc />
    public bool Equals(MessageHandlerCallback<ICommandMessage<object>>? other) => this.callback.Equals(other);

    /// <inheritdoc />
    public bool Equals(WrappedCommandHandlerCallback? other)
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

        return this.Equals((WrappedCommandHandlerCallback)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode() => this.callback.GetHashCode();
}
