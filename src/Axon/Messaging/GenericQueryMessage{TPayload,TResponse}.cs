namespace Axon.Messaging;

using Axon.Messaging.ResponseTypes;

/// <summary>
/// Generic implementation of the QueryMessage.
/// </summary>
/// <typeparam name="TPayload">The type of payload expressing the query in this message.</typeparam>
/// <typeparam name="TResponse">The type of response expected from this query.</typeparam>
public class GenericQueryMessage<TPayload, TResponse> : MessageDecorator<TPayload>, IQueryMessage<TPayload, TResponse>
    where TPayload : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericQueryMessage{TPayload, TResponse}"/> class using the given
    /// <paramref name="payload"/> and expected <paramref name="responseType"/>.
    /// The query name is set to the fully qualified class name of the <paramref name="payload"/>.
    /// </summary>
    /// <param name="payload">The payload expressing the query.</param>
    /// <param name="responseType">The expected response type of type <see cref="IResponseType{TResponse}"/>.</param>
    public GenericQueryMessage(TPayload payload, IResponseType<TResponse> responseType)
        : this(payload, payload.GetType().FullName!, responseType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericQueryMessage{TPayload, TResponse}"/> class using the given
    /// <paramref name="payload"/>, <paramref name="queryName"/> and expected <paramref name="responseType"/>.
    /// </summary>
    /// <param name="payload">The payload expressing the query.</param>
    /// <param name="queryName">The name identifying the query to execute.</param>
    /// <param name="responseType">The expected response type of type <see cref="IResponseType{TResponse}"/>.</param>
    public GenericQueryMessage(TPayload payload, string queryName, IResponseType<TResponse> responseType)
        : this(new GenericMessage<TPayload>(payload), queryName, responseType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericQueryMessage{TPayload, TResponse}"/> class the given
    /// <paramref name="message"/> as the carrier of payload and metadata and given <paramref name="queryName"/> and
    /// expecting the given <paramref name="responseType"/>.
    /// </summary>
    /// <param name="message">The message containing the payload and meta data for this message.</param>
    /// <param name="queryName">The name identifying the query to execute.</param>
    /// <param name="responseType">The expected response type of type <see cref="IResponseType{TResponse}"/>.</param>
    public GenericQueryMessage(IMessage<TPayload> message, string queryName, IResponseType<TResponse> responseType)
        : base(message)
    {
        this.QueryName = queryName;
        this.ResponseType = responseType;
    }

    /// <inheritdoc />
    public string QueryName { get; }

    /// <inheritdoc />
    public IResponseType<TResponse> ResponseType { get; }

    /// <inheritdoc />
    public override IMessage<TPayload> WithMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        new GenericQueryMessage<TPayload, TResponse>(
            this.Message.WithMetaData(metaData),
            this.QueryName,
            this.ResponseType);

    /// <inheritdoc />
    public override IMessage<TPayload> AndMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        new GenericQueryMessage<TPayload, TResponse>(
            this.Message.AndMetaData(metaData),
            this.QueryName,
            this.ResponseType);
}
