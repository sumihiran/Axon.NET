namespace Axon;

using Axon.Messaging.ResponseTypes;

/// <summary>
/// Interface for identifying the fields of a Query Handler when one is subscribed to the QueryBus.
/// <para/>
/// </summary>
public interface IQuerySubscription
{
    /// <summary>
    /// Gets the response type of this subscription.
    /// </summary>
    Type ResponseType { get; }

    /// <summary>
    /// Gets the query handler of this subscription as a <see cref="IMessageHandler{TMessage}"/>.
    /// </summary>
    object QueryHandler { get; }

    /// <summary>
    /// Check if this <see cref="QuerySubscription{TResponse}"/> can handle the given
    /// <paramref name="queryResponseType"/>, by calling the <see cref="IResponseType{TResponse}.Matches(Type)"/>
    /// function on it and providing the <see cref="QuerySubscription{TResponse}.ResponseType"/> of this subscription.
    /// </summary>
    /// <param name="queryResponseType">The queryResponseType: a <see cref="IResponseType{TResponse}"/> to match this
    /// subscriptions it's <see cref="QuerySubscription{TResponse}.ResponseType"/> against.</param>
    /// <returns>
    /// <c>true</c> if the given <paramref name="queryResponseType"/> its <see cref="IResponseType{TResponse}.Matches(Type)"/>
    /// returns <c>true</c>, <c>false</c> if otherwise.
    /// </returns>
    bool CanHandle(IResponseType<object> queryResponseType);
}
