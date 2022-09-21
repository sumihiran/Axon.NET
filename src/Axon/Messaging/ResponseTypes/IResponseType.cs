namespace Axon.Messaging.ResponseTypes;

/// <summary>
/// Specifies the expected response type required when performing a query through the <see cref="IQueryBus"/>.
/// </summary>
/// <typeparam name="TResponse">
/// The generic type of this <see cref="IResponseType{TResponse}"/> to be matched and converted.
/// </typeparam>
public interface IResponseType<out TResponse>
{
    /// <summary>
    /// Gets the <see cref="Type"/> of the payload to be contained in the response message.
    /// </summary>
    Type ResponseMessagePayloadType { get; }

    /// <summary>
    /// Gets actual response type or generic placeholder.
    /// </summary>
    Type ExpectedResponseType { get; }

    /// <summary>
    /// Match the query handler with its response <see cref="Type"/> with the <see cref="IResponseType{TResponse}"/>
    /// implementation its expected response type <typeparamref name="TResponse"/>.
    /// Will return <c>true</c> if a response can be converted based on the given <paramref name="responseType"/> and
    /// <c>false</c> if it cannot.
    /// </summary>
    /// <param name="responseType">
    /// The response <see cref="Type"/> of the query handler which is matched against.
    /// </param>
    /// <returns><c>true</c> if a response can be converted based on the given <paramref name="responseType"/> and
    /// <c>false</c> if it cannot.</returns>
    bool Matches(Type responseType);

    /// <summary>
    /// Converts the given <paramref name="response"/> of type <see cref="object"/> into the type
    /// <typeparamref name="TResponse"/> of this <see cref="IResponseType{TResponse}"/> instance. Should only be called
    /// if <see cref="IResponseType{TResponse}.Matches(Type)"/> returns <c>true</c>. It is unspecified what this
    /// function does if the <see cref="IResponseType{TResponse}.Matches(Type)"/> returned <c>false</c>.
    /// </summary>
    /// <param name="response">
    /// The response of type <see cref="object"/> to convert into <typeparamref name="TResponse"/>.
    /// </param>
    /// <returns>A <paramref name="response"/> of type <typeparamref name="TResponse"/>.</returns>
    public TResponse? Convert(object? response) => (TResponse?)response;
}
