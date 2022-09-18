namespace Axon.Messaging;

/// <summary>
/// Implementation of the CommandMessage that takes all properties as constructor parameters.
/// </summary>
/// <typeparam name="TPayload">The type of payload contained in this Message.</typeparam>
public class GenericCommandMessage<TPayload> : MessageDecorator<TPayload>, ICommandMessage<TPayload>
    where TPayload : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericCommandMessage{TPayload}"/> class with the given
    /// <paramref name="payload"/> and empty metaData.
    /// </summary>
    /// <param name="payload">The payload for the Message.</param>
    public GenericCommandMessage(TPayload payload)
        : this(payload, MetaData.EmptyInstance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericCommandMessage{TPayload}"/> class from the given
    /// <paramref name="payload"/>.
    /// </summary>
    /// <param name="payload">The payload for the Message.</param>
    /// <param name="metaData">The metadata for this message.</param>
    public GenericCommandMessage(TPayload payload, ICollection<KeyValuePair<string, object>> metaData)
        : this(new GenericMessage<TPayload>(payload, metaData), payload.GetType().FullName!)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericCommandMessage{TPayload}"/> class from the given
    /// <paramref name="message"/> containing payload, metadata and message identifier, and the given
    /// <paramref name="commandName"/>.
    /// </summary>
    /// <param name="message">The message containing payload, identifier and metadata.</param>
    /// <param name="commandName">The name of the command.</param>
    public GenericCommandMessage(IMessage<TPayload> message, string commandName)
        : base(message) => this.CommandName = commandName;

    /// <inheritdoc />
    public string CommandName { get; }

    /// <inheritdoc />
    public override IMessage<TPayload> WithMetaData(ICollection<KeyValuePair<string, object>> metaData)
        => new GenericCommandMessage<TPayload>(this.Message.WithMetaData(this.MetaData), this.CommandName);

    /// <inheritdoc />
    public override IMessage<TPayload> AndMetaData(ICollection<KeyValuePair<string, object>> metaData)
        => new GenericCommandMessage<TPayload>(this.Message.AndMetaData(this.MetaData), this.CommandName);
}
