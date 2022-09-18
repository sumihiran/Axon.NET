namespace Axon.Messaging;

/// <summary>
/// Non-generic static members for GenericMessage.
/// </summary>
public static class GenericMessage
{
    private static readonly Type GenericMessageType = typeof(GenericMessage<>);

    /// <summary>
    /// Returns a Message representing the given <paramref name="payload"/>, either by wrapping it or
    /// by returning it as-is.
    /// </summary>
    /// <param name="payload">The payload to wrap or message to return.</param>
    /// <returns>A Message with the given payload or the message.</returns>
    public static IMessage<object> AsMessage(object? payload)
    {
        if (payload is null)
        {
            return new GenericMessage<object>(payload);
        }

        if (payload is IMessage<object> message)
        {
            return message;
        }

        var messageType = GenericMessageType.MakeGenericType(payload.GetType());
        return (IMessage<object>)Activator.CreateInstance(messageType, args: new[] { payload })!;
    }

    /// <summary>
    /// Extract the <see cref="Type"/> of the provided <paramref name="payload"/>.
    /// If {<paramref name="payload"/> == null}, this function returns <see cref="Void"/> as the payload type.
    /// </summary>
    /// <param name="payload">The payload of a <see cref="IMessage{TPayload}"/>.</param>
    /// <typeparam name="TPayload">The type of payload.</typeparam>
    /// <returns>The declared type of the given <paramref name="payload"/> or <see cref="Void"/> if
    /// {<paramref name="payload"/> == null}.</returns>
    internal static Type GetDeclaredPayloadType<TPayload>(TPayload? payload)
        where TPayload : class =>
        payload is null ? typeof(void) : typeof(TPayload);
}
