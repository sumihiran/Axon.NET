namespace Axon.Messaging;

/// <summary>
/// Non-generic static members for GenericMessage.
/// </summary>
public static class GenericMessage
{
    /// <summary>
    /// Returns a Message representing the given <paramref name="payloadOrMessage"/>, either by wrapping it or
    /// by returning it as-is.
    /// </summary>
    /// <param name="payloadOrMessage">The payload to wrap or message to return.</param>
    /// <returns>A Message with the given payload or the message.</returns>
    public static IMessage<object> AsMessage(object payloadOrMessage)
    {
        if (payloadOrMessage is IMessage<object> message)
        {
            return message;
        }

        var constructedClass = typeof(GenericMessage<>).MakeGenericType(payloadOrMessage.GetType());
        return (IMessage<object>)Activator.CreateInstance(constructedClass, args: new[] { payloadOrMessage })!;
    }
}
