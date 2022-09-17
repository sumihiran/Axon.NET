namespace Axon.Messaging;

/// <summary>
/// Abstract implementation of a {@link Message} that delegates to an existing message. Extend this decorator class to
/// extend the message with additional features.
/// </summary>
/// <typeparam name="TPayload">The type of payload contained in this Message.</typeparam>
public abstract class MessageDecorator<TPayload> : IMessage<TPayload>
    where TPayload : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDecorator{TPayload}"/> class.
    /// </summary>
    /// <param name="message">The message delegate.</param>
    public MessageDecorator(IMessage<TPayload> message) => this.Message = message;

    /// <summary>
    /// Gets the wrapped message.
    /// </summary>
    public IMessage<TPayload> Message { get; }

    /// <inheritdoc />
    public string Identifier => this.Message.Identifier;

    /// <inheritdoc />
    public MetaData MetaData => this.Message.MetaData;

    /// <inheritdoc />
    public TPayload Payload => this.Message.Payload;

    /// <inheritdoc />
    public Type PayloadType => this.Message.PayloadType;

    /// <inheritdoc />
    public abstract IMessage<TPayload> WithMetaData(ICollection<KeyValuePair<string, object>> metaData);

    /// <inheritdoc />
    public abstract IMessage<TPayload> AndMetaData(ICollection<KeyValuePair<string, object>> metaData);
}
