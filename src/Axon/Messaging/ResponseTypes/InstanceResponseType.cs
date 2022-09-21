namespace Axon.Messaging.ResponseTypes;

/// <summary>
/// A <see cref="IResponseType{TResponse}"/> implementation that will match with query handlers, which return a single
/// instance of the expected response type.
/// </summary>
/// <typeparam name="TResponse">The response type that is supposed to be matched against and converted to.</typeparam>
public class InstanceResponseType<TResponse> : IResponseType<TResponse>
{
    /// <inheritdoc />
    public Type ResponseMessagePayloadType => typeof(TResponse);

    /// <inheritdoc />
    public Type ExpectedResponseType => typeof(TResponse);

    /// <inheritdoc />
    public bool Matches(Type responseType) => responseType == this.ResponseMessagePayloadType;

    /// <inheritdoc />
    public override string ToString() => $"InstanceResponseType{this.ExpectedResponseType}";
}
