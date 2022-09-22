namespace Axon.Messaging;

using Axon.Messaging.ResponseTypes;

/// <summary>
/// Message type that carries a Query: a request for information. Besides a payload, Query Messages also carry the
/// expected response type. This is the type of result expected by the caller.
/// <para/>
/// Handlers should only answer a query if they can respond with the appropriate response type.
/// </summary>
/// <typeparam name="TPayload">The type of payload.</typeparam>
/// <typeparam name="TResponse">The type of response expected from this query.</typeparam>
public interface IQueryMessage<out TPayload, out TResponse> : IMessage<TPayload>
    where TPayload : class
    where TResponse : class
{
    /// <summary>
    /// Gets the name identifying the query to be executed.
    /// </summary>
    string QueryName { get; }

    /// <summary>
    /// Gets the type of response expected by the sender of the query.
    /// </summary>
    IResponseType<TResponse> ResponseType { get; }
}
