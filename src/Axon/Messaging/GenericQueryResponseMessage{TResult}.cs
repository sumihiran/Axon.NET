namespace Axon.Messaging;

/// <summary>
/// QueryResponseMessage implementation that takes all properties as constructor parameters.
/// </summary>
/// <typeparam name="TResult">The type of return value contained in this response.</typeparam>
public class GenericQueryResponseMessage<TResult> : GenericResultMessage<TResult>, IQueryResponseMessage<TResult>
    where TResult : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericQueryResponseMessage{TResult}"/> class with
    /// <typeparamref name="TResult"/> as declaredResultType and a <c>null</c> result.
    /// </summary>
    public GenericQueryResponseMessage()
        : this(typeof(TResult), default(TResult), MetaData.EmptyInstance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericQueryResponseMessage{TResult}"/> class with given
    /// <paramref name="result"/>. This constructor does not allow the actual result to be <c>null</c>.
    /// </summary>
    /// <param name="result">The actual result. May be <c>null</c>.</param>
    public GenericQueryResponseMessage(TResult result)
        : this(typeof(TResult), result, MetaData.EmptyInstance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericQueryResponseMessage{TResult}"/> class with given
    /// <paramref name="result"/> and <paramref name="declaredResultType"/>. This constructor allows the actual result
    /// to be <c>null</c>.
    /// </summary>
    /// <param name="declaredResultType">The declared type of the result.</param>
    /// <param name="result">The actual result. May be <c>null</c>.</param>
    public GenericQueryResponseMessage(Type declaredResultType, TResult? result)
        : this(declaredResultType, result, MetaData.EmptyInstance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericQueryResponseMessage{TResult}"/> class with given
    /// <paramref name="declaredResultType"/> and <paramref name="exception"/>.
    /// </summary>
    /// <param name="declaredResultType">The declared type of the Query Response Message to be created.</param>
    /// <param name="exception">The Exception describing the cause of an error.</param>
    public GenericQueryResponseMessage(Type declaredResultType, Exception exception)
        : this(declaredResultType, exception, MetaData.EmptyInstance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericQueryResponseMessage{TResult}"/> class with given
    /// <paramref name="result"/> and <paramref name="metaData"/>.
    /// </summary>
    /// <param name="result">The result reported by the Query Handler.</param>
    /// <param name="metaData">The meta data to contain in the message.</param>
    public GenericQueryResponseMessage(TResult result, MetaData metaData)
        : base(new GenericMessage<TResult>(result, metaData))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericQueryResponseMessage{TResult}"/> class with a specific
    /// <paramref name="declaredResultType"/>, the given <paramref name="result"/> as payload and
    /// <paramref name="metaData"/>.
    /// </summary>
    /// <param name="declaredResultType">The declared type of the Query Response Message to be created.</param>
    /// <param name="result">The result reported by the Query Handler.</param>
    /// <param name="metaData">The meta data to contain in the message.</param>
    public GenericQueryResponseMessage(Type declaredResultType, TResult? result, MetaData metaData)
        : base(new GenericMessage<TResult>(declaredResultType, result, metaData))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericQueryResponseMessage{TResult}"/> class with a specific
    /// <paramref name="declaredResultType"/>, <paramref name="exception"/> and <paramref name="metaData"/>.
    /// </summary>
    /// <param name="declaredResultType">The declared type of the Query Response Message to be created.</param>
    /// <param name="exception">The Exception describing the cause of an error.</param>
    /// <param name="metaData">The meta data to contain in the message.</param>
    public GenericQueryResponseMessage(Type declaredResultType, Exception exception, MetaData metaData)
        : base(new GenericMessage<TResult>(declaredResultType, default, metaData), exception)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericQueryResponseMessage{TResult}"/> class by taking the
    /// payload, meta data and message identifier of the given <paramref name="message"/> for this message.
    /// </summary>
    /// <param name="message">The message to retrieve message details from.</param>
    public GenericQueryResponseMessage(IMessage<TResult> message)
        : base(message)
    {
    }

    /// <inheritdoc />
    IQueryResponseMessage<TResult> IQueryResponseMessage<TResult>.
        WithMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        this.WithMetaData(metaData);

    /// <inheritdoc />
    IQueryResponseMessage<TResult> IQueryResponseMessage<TResult>.AndMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        this.AndMetaData(metaData);

    /// <inheritdoc />
    public override GenericQueryResponseMessage<TResult> WithMetaData(
        ICollection<KeyValuePair<string, object>> metaData) => new(this.Message.WithMetaData(metaData));

    /// <inheritdoc />
    public override GenericQueryResponseMessage<TResult>
        AndMetaData(ICollection<KeyValuePair<string, object>> metaData) => new(this.Message.AndMetaData(metaData));
}
