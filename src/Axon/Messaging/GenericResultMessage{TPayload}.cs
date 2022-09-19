namespace Axon.Messaging;

/// <summary>
/// Generic implementation of <see cref="IResultMessage{TPayload}"/>.
/// </summary>
/// <typeparam name="TPayload">The type of payload contained in this ResultMessage.</typeparam>
public class GenericResultMessage<TPayload> : MessageDecorator<TPayload>, IResultMessage<TPayload>
    where TPayload : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericResultMessage{TPayload}"/> class with the given
    /// <paramref name="result"/> as the payload.
    /// </summary>
    /// <param name="result">The payload for the Message.</param>
    public GenericResultMessage(TPayload? result)
        : this(result, MetaData.EmptyInstance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericResultMessage{TPayload}"/> class with the given
    /// <paramref name="exception"/>.
    /// </summary>
    /// <param name="exception">The Exception describing the cause of an error.</param>
    public GenericResultMessage(Exception exception)
        : this(exception, MetaData.EmptyInstance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericResultMessage{TPayload}"/> class with the given
    /// <paramref name="result"/> as the payload and <paramref name="metaData"/> as the metadata.
    /// </summary>
    /// <param name="result">The payload for the Message.</param>
    /// <param name="metaData">The metadata for the message.</param>
    public GenericResultMessage(TPayload? result, MetaData metaData)
        : this(new GenericMessage<TPayload>(result, metaData))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericResultMessage{TPayload}"/> class with the given
    /// <paramref name="exception"/> and <paramref name="metaData"/> as the metadata.
    /// </summary>
    /// <param name="exception">The Exception describing the cause of an error.</param>
    /// <param name="metaData">The metadata for the message.</param>
    public GenericResultMessage(Exception exception, MetaData metaData)
        : this(new GenericMessage<TPayload>(null, metaData), exception)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericResultMessage{TPayload}"/> class with given
    /// <paramref name="message"/> as delegate message.
    /// </summary>
    /// <param name="message">The message delegate.</param>
    public GenericResultMessage(IMessage<TPayload> message)
        : this(message, GenericResultMessage.FindResultException(message)) =>
        this.Exception = GenericResultMessage.FindResultException(message);

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericResultMessage{TPayload}"/> class with given
    /// <paramref name="message"/> as delegate message and <paramref name="exception"/>.
    /// </summary>
    /// <param name="message">The Message delegate.</param>
    /// <param name="exception">The Exception describing the cause of an error.</param>
    public GenericResultMessage(IMessage<TPayload> message, Exception? exception)
        : base(message) =>
        this.Exception = exception;

    /// <inheritdoc />
    public bool IsSuccess => this.Exception == null;

    /// <inheritdoc />
    public Exception? Exception { get; }

    /// <inheritdoc />
    public override GenericResultMessage<TPayload> WithMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        new(this.Message.WithMetaData(metaData), this.Exception);

    /// <inheritdoc />
    public override GenericResultMessage<TPayload> AndMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        new(this.Message.AndMetaData(metaData), this.Exception);
}
