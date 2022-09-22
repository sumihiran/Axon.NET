namespace Axon.Messaging;

/// <summary>
/// Generic implementation of <see cref="ICommandResultMessage{TResult}"/>.
/// </summary>
/// <typeparam name="TResult">The type of the payload contained in returned Message.</typeparam>
public class GenericCommandResultMessage<TResult> : GenericResultMessage<TResult>, ICommandResultMessage<TResult>
    where TResult : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericCommandResultMessage{TResult}"/> class with the given
    /// <paramref name="commandResult"/> as the payload.
    /// </summary>
    /// <param name="commandResult">The payload for the Message.</param>
    public GenericCommandResultMessage(TResult? commandResult)
        : base(commandResult)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericCommandResultMessage{TResult}"/> class with the given
    /// <paramref name="exception"/>.
    /// </summary>
    /// <param name="exception">The Exception describing the cause of an error.</param>
    public GenericCommandResultMessage(Exception exception)
        : base(exception)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericCommandResultMessage{TResult}"/> class with the given
    /// <paramref name="commandResult"/> as the payload and <paramref name="metaData"/> as metadata.
    /// </summary>
    /// <param name="commandResult">The payload for the Message.</param>
    /// <param name="metaData">The meta data for the Message.</param>
    public GenericCommandResultMessage(TResult? commandResult, MetaData metaData)
        : base(commandResult, metaData)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericCommandResultMessage{TResult}"/> class with the given
    /// <paramref name="exception"/> and <paramref name="metaData"/> as metadata.
    /// </summary>
    /// <param name="exception">The Exception describing the cause of an error.</param>
    /// <param name="metaData">The meta data for the Message.</param>
    public GenericCommandResultMessage(Exception exception, MetaData metaData)
        : base(exception, metaData)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericCommandResultMessage{TResult}"/> class with given
    /// <paramref name="message"/> message.
    /// </summary>
    /// <param name="message">the message delegate.</param>
    public GenericCommandResultMessage(IMessage<TResult> message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericCommandResultMessage{TResult}"/> class with given
    /// <paramref name="message"/> message and <paramref name="exception"/>.
    /// </summary>
    /// <param name="message">the message delegate.</param>
    /// <param name="exception">The Exception describing the cause of an error.</param>
    public GenericCommandResultMessage(IMessage<TResult> message, Exception? exception)
        : base(message, exception)
    {
    }

    /// <inheritdoc />
    ICommandResultMessage<TResult> ICommandResultMessage<TResult>.
        WithMetaData(ICollection<KeyValuePair<string, object>> metaData) => this.WithMetaData(metaData);

    /// <inheritdoc />
    ICommandResultMessage<TResult> ICommandResultMessage<TResult>
        .AndMetaData(ICollection<KeyValuePair<string, object>> metaData) => this.AndMetaData(metaData);

    /// <inheritdoc />
    public override GenericCommandResultMessage<TResult> WithMetaData(
        ICollection<KeyValuePair<string, object>> metaData) => new(this.Message.WithMetaData(metaData), this.Exception);

    /// <inheritdoc />
    public override GenericCommandResultMessage<TResult> AndMetaData(ICollection<KeyValuePair<string, object>> metaData)
        => new(this.Message.AndMetaData(metaData), this.Exception);
}
