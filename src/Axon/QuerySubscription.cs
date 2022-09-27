namespace Axon;

using Axon.Messaging;
using Axon.Messaging.ResponseTypes;

/// <summary>
/// Encapsulates the identifying fields of a Query Handler when one is subscribed to the <see cref="IQueryBus"/>.
/// As such contains the response type of the query handler and the complete handler itself. The first is typically used
/// by the QueryBus to select the right query handler when a query comes in. The latter is used to perform the actual
/// query.
/// </summary>
/// <typeparam name="TResponse">The type of response this query subscription contains.</typeparam>
public class QuerySubscription<TResponse> :
    IEquatable<QuerySubscription<TResponse>>, IQuerySubscription
    where TResponse : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuerySubscription{TResponse}"/> class  with a specific
    /// <paramref name="responseType"/> and <paramref name="queryHandler"/>.
    /// </summary>
    /// <param name="responseType">This subscription's response <see cref="Type"/>.</param>
    /// <param name="queryHandler">The subscribed <see cref="IMessageHandler{TMessage}"/>.</param>
    public QuerySubscription(Type responseType, IMessageHandler<IQueryMessage<object, TResponse>> queryHandler)
    {
        this.ResponseType = responseType;
        this.QueryHandler = queryHandler;
    }

    /// <summary>
    /// Gets the response type of this subscription.
    /// </summary>
    public Type ResponseType { get; }

    /// <inheritdoc/>
    object IQuerySubscription.QueryHandler => this.QueryHandler;

    /// <summary>
    /// Gets the query handler of this subscription as a <see cref="IMessageHandler{TMessage}"/>.
    /// </summary>
    public IMessageHandler<IQueryMessage<object, TResponse>> QueryHandler { get; }

    /// <inheritdoc />
    public bool CanHandle(IResponseType<object> queryResponseType) => queryResponseType.Matches(this.ResponseType);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(this.QueryHandler, this.ResponseType);

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

        return obj.GetType() == this.GetType() && this.Equals((QuerySubscription<TResponse>)obj);
    }

    /// <inheritdoc />
    public bool Equals(QuerySubscription<TResponse>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.QueryHandler.Equals(other.QueryHandler) &&
               this.ResponseType == other.ResponseType;
    }
}
