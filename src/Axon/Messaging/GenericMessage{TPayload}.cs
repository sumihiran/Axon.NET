namespace Axon.Messaging;

/// <summary>
/// Generic implementation of a <see cref="IMessage{TPayload}"/> that contains the payload and metadata.
/// </summary>
/// <typeparam name="TPayload">The type of payload contained in this Message.</typeparam>
public class GenericMessage<TPayload> : AbstractMessage<TPayload>
    where TPayload : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericMessage{TPayload}"/> class for the given
    /// <paramref name="payload"/>.
    /// </summary>
    /// <param name="payload">The payload for the message.</param>
    public GenericMessage(TPayload? payload)
        : this(payload, MetaData.EmptyInstance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericMessage{TPayload}"/> class for the given
    /// <paramref name="payload"/> and <paramref name="metaData"/>.
    /// </summary>
    /// <param name="payload">The payload for the message as a generic <typeparamref name="TPayload"/>.</param>
    /// <param name="metaData">The metadata <see cref="ICollection{T}"/> for the message.</param>
    public GenericMessage(TPayload? payload, ICollection<KeyValuePair<string, object>> metaData)
        : this(GenericMessage.GetDeclaredPayloadType(payload), payload, metaData)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericMessage{TPayload}"/> class for the given
    /// <paramref name="payload"/> and <paramref name="metaData"/>.
    /// </summary>
    /// <param name="declaredPayloadType">The declared type of message payload.</param>
    /// <param name="payload">The payload for the message as a generic <typeparamref name="TPayload"/>.</param>
    /// <param name="metaData">The metadata <see cref="ICollection{T}"/> for the message.</param>
    public GenericMessage(
        Type declaredPayloadType,
        TPayload? payload,
        ICollection<KeyValuePair<string, object>> metaData)
        : this(Guid.NewGuid().ToString("D"), declaredPayloadType, payload, metaData)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericMessage{TPayload}"/> class for the given
    /// <paramref name="payload"/> and <paramref name="metaData"/>.
    /// </summary>
    /// <param name="identifier">The identifier of the Message.</param>
    /// <param name="payload">The payload for the message.</param>
    /// <param name="metaData">The MetaData for the message.</param>
    public GenericMessage(string identifier, TPayload? payload, ICollection<KeyValuePair<string, object>> metaData)
        : this(identifier, GenericMessage.GetDeclaredPayloadType(payload), payload, metaData)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericMessage{TPayload}"/> class using existing Message data.
    /// </summary>
    /// <param name="identifier">The identifier of the Message.</param>
    /// <param name="declaredPayloadType">The declared type of message payload.</param>
    /// <param name="payload">The payload for the message.</param>
    /// <param name="metaData">The MetaData for the message.</param>
    public GenericMessage(
        string identifier,
        Type declaredPayloadType,
        TPayload? payload,
        ICollection<KeyValuePair<string, object>> metaData)
        : base(identifier)
    {
        this.Payload = payload;
        this.PayloadType = declaredPayloadType;
        this.MetaData = MetaData.From(metaData);
    }

    private GenericMessage(GenericMessage<TPayload> original, MetaData metaData)
        : base(original.Identifier)
    {
        this.Payload = original.Payload;
        this.PayloadType = original.PayloadType;
        this.MetaData = metaData;
    }

    /// <inheritdoc />
    public override MetaData MetaData { get; }

    /// <inheritdoc />
    public override TPayload? Payload { get; }

    /// <inheritdoc />
    public override Type PayloadType { get; }

    /// <inheritdoc />
    protected override IMessage<TPayload> WithMetaData(MetaData metaData) =>
        new GenericMessage<TPayload>(this, metaData);
}
