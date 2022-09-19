namespace Axon.Messaging.ResponseTypes;

/// <summary>
/// Utility class containing static methods to obtain instances of <see cref="IResponseType{TResponse}"/>.
/// </summary>
public static class ResponseTypes
{
    /// <summary>
    /// Specify the desire to retrieve a single instance of type <typeparamref name="TResponse"/> when performing a query.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The generic type of the instantiated <see cref="IResponseType{TResponse}"/>.
    /// </typeparam>
    /// <returns>
    /// A <see cref="IResponseType{TResponse}"/> specifying the desire to retrieve a single instance of type.
    /// </returns>
    public static IResponseType<TResponse> InstanceOf<TResponse>() => new InstanceResponseType<TResponse>();
}
