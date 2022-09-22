namespace Axon.Messaging;

/// <summary>
/// Generic implementation of the EventMessage interface.
/// </summary>
/// <typeparam name="TPayload">The type of payload contained in this Message.</typeparam>
public class GenericEventMessage<TPayload> : MessageDecorator<TPayload>, IEventMessage<TPayload>
    where TPayload : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericEventMessage{TPayload}"/> class with given
    /// <paramref name="payload"/>, and an empty MetaData.
    /// </summary>
    /// <param name="payload">The payload for the message.</param>
    /// <seealso cref="GenericEventMessage.AsEventMessage{TMessage}(TMessage, Func{DateTime}?)"/>
    public GenericEventMessage(TPayload payload)
        : this(payload, MetaData.EmptyInstance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericEventMessage{TPayload}"/> class with given
    /// <paramref name="payload"/>, and given <paramref name="metaData"/>.
    /// </summary>
    /// <param name="payload">The payload of the EventMessage.</param>
    /// <param name="metaData">The MetaData for the EventMessage.</param>
    public GenericEventMessage(TPayload payload, ICollection<KeyValuePair<string, object>> metaData)
        : this(new GenericMessage<TPayload>(payload, metaData), GenericEventMessage.SystemTimestampSupplier)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericEventMessage{TPayload}"/> class using an
    /// existing EventMessage.
    /// </summary>
    /// <param name="message">The message containing payload, identifier and metadata.</param>
    public GenericEventMessage(IMessage<TPayload> message)
        : this(message, GenericEventMessage.SystemTimestampSupplier)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericEventMessage{TPayload}"/> class using an existing
    /// EventMessage data.
    /// The timestamp of the event is supplied lazily to prevent unnecessary deserialization of the timestamp.
    /// </summary>
    /// <param name="message">The message containing payload, identifier and metadata.</param>
    /// <param name="timestampSupplier">Supplier for the timestamp of the Message creation.</param>
    public GenericEventMessage(IMessage<TPayload> message, Func<DateTime> timestampSupplier)
        : base(message) => this.Timestamp = timestampSupplier();

    /// <inheritdoc />
    public DateTime Timestamp { get; }

    /// <inheritdoc />
    IEventMessage<TPayload> IEventMessage<TPayload>.
        WithMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        this.WithMetaData(metaData);

    /// <inheritdoc />
    IEventMessage<TPayload> IEventMessage<TPayload>.AndMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        this.AndMetaData(metaData);

    /// <inheritdoc />
    public override GenericEventMessage<TPayload> WithMetaData(ICollection<KeyValuePair<string, object>> metaData)
    {
        if (this.MetaData.Equals(metaData))
        {
            return this;
        }

        return new GenericEventMessage<TPayload>(this.Message.WithMetaData(metaData), () => this.Timestamp);
    }

    /// <inheritdoc />
    public override GenericEventMessage<TPayload> AndMetaData(ICollection<KeyValuePair<string, object>> metaData)
    {
        if (metaData.IsEmpty() || this.MetaData.Equals(metaData))
        {
            return this;
        }

        return new GenericEventMessage<TPayload>(this.Message.AndMetaData(metaData), () => this.Timestamp);
    }
}
