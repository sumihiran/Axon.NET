namespace Axon.Messaging;

/// <summary>
/// Abstract base class for Messages.
/// </summary>
/// <typeparam name="TPayload">The type of payload contained in this Message.</typeparam>
public abstract class AbstractMessage<TPayload> : IMessage<TPayload>
    where TPayload : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractMessage{T}"/> class  with given identifier.
    /// </summary>
    /// <param name="identifier">The message identifier.</param>
    protected AbstractMessage(string identifier) => this.Identifier = identifier;

    /// <inheritdoc />
    public string Identifier { get; }

    /// <inheritdoc />
    public abstract MetaData MetaData { get; }

    /// <inheritdoc />
    public abstract TPayload? Payload { get; }

    /// <inheritdoc />
    public abstract Type PayloadType { get; }

    /// <inheritdoc />
    public IMessage<TPayload> WithMetaData(ICollection<KeyValuePair<string, object>> metaData)
    {
        if (this.MetaData.Equals(metaData))
        {
            return this;
        }

        return this.WithMetaData(MetaData.From(metaData));
    }

    /// <inheritdoc />
    public IMessage<TPayload> AndMetaData(ICollection<KeyValuePair<string, object>> metaData)
    {
        if (metaData.IsEmpty())
        {
            return this;
        }

        return this.WithMetaData(this.MetaData.MergedWith(metaData));
    }

    /// <summary>
    /// Returns a new message instance with the same payload and properties as this message but given
    /// <paramref name="metaData"/>.
    /// </summary>
    /// <param name="metaData">The metadata in the new message.</param>
    /// <returns>A  copy of this instance with given metadata.</returns>
    protected abstract IMessage<TPayload> WithMetaData(MetaData metaData);
}
