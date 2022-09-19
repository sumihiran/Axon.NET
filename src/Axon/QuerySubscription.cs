namespace Axon;

using Axon.Messaging;
using Axon.Messaging.ResponseTypes;

/// <summary>
/// Encapsulates the identifying fields of a Query Handler when one is subscribed to the <see cref="IQueryBus"/>.
/// </summary>
public class QuerySubscription : IEquatable<QuerySubscription>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuerySubscription"/> class  with a specific
    /// <paramref name="responseType"/> and <paramref name="queryHandler"/>.
    /// </summary>
    /// <param name="responseType">This subscription's response <see cref="Type"/>.</param>
    /// <param name="queryHandler">The subscribed <see cref="IMessageHandler"/>.</param>
    public QuerySubscription(Type responseType, IMessageHandler queryHandler)
    {
        this.QueryHandler = queryHandler;
        this.ResponseType = responseType;
    }

    /// <summary>
    /// Gets the response type of this subscription.
    /// </summary>
    public Type ResponseType { get; }

    /// <summary>
    /// Gets the query handler of this subscription as a <see cref="IMessageHandler"/>.
    /// </summary>
    public IMessageHandler QueryHandler { get; }

    /// <summary>
    /// Check if this <see cref="QuerySubscription"/> can handle the given <paramref name="queryResponseType"/>, by
    /// calling the <see cref="IResponseType{TResponse}.Matches(Type)"/> function on it and providing the
    /// <see cref="ResponseType"/> of this subscription.
    /// </summary>
    /// <param name="queryResponseType">The queryResponseType: a <see cref="IResponseType{TResponse}"/> to match this
    /// subscriptions it's <see cref="ResponseType"/> against.</param>
    /// <returns>
    /// <c>true</c> if the given <paramref name="queryResponseType"/> its <see cref="IResponseType{TResponse}.Matches(Type)"/>
    /// returns <c>true</c>, <c>false</c> if otherwise.
    /// </returns>
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

        return obj.GetType() == this.GetType() && this.Equals((QuerySubscription)obj);
    }

    /// <inheritdoc />
    public bool Equals(QuerySubscription? other)
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
