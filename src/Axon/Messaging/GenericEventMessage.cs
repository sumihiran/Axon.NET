namespace Axon.Messaging;

/// <summary>
/// Non-generic static members for GenericEventMessage.
/// </summary>
public static class GenericEventMessage
{
    /// <summary>
    /// Default timestamp supplier using system DateTime.
    /// </summary>
    public static readonly Func<DateTime> SystemTimestampSupplier = () => DateTime.UtcNow;

    /// <summary>
    /// Returns the given event as an EventMessage. If <paramref name="eventMessage"/> already implements EventMessage,
    /// it is returned as-is. If it is a Message, a new EventMessage will be created using the payload and metadata
    /// of the given message.
    /// Otherwise, the given <paramref name="eventMessage"/> is wrapped into a GenericEventMessage as its payload.
    /// </summary>
    /// <param name="eventMessage">The event to wrap as EventMessage.</param>
    /// <param name="timestampSupplier">The timestamp of the event message.</param>
    /// <typeparam name="TMessage">The generic type of the expected payload of the resulting object.</typeparam>
    /// <returns>
    /// An EventMessage containing given <paramref name="eventMessage"/> as payload, or <paramref name="eventMessage"/>
    /// if it already implements EventMessage.
    /// </returns>
    public static IEventMessage<TMessage> AsEventMessage<TMessage>(
        TMessage eventMessage,
        Func<DateTime>? timestampSupplier = null)
        where TMessage : class => AsEventMessage<TMessage>((object)eventMessage, timestampSupplier);

    /// <summary>
    /// Returns the given event as an EventMessage. If <paramref name="eventMessage"/> already implements EventMessage,
    /// it is returned as-is. If it is a Message, a new EventMessage will be created using the payload and metadata
    /// of the given message.
    /// Otherwise, the given <paramref name="eventMessage"/> is wrapped into a GenericEventMessage as its payload.
    /// </summary>
    /// <param name="eventMessage">The event to wrap as EventMessage.</param>
    /// <param name="timestampSupplier">The timestamp of the event message.</param>
    /// <typeparam name="TMessage">The generic type of the expected payload of the resulting object.</typeparam>
    /// <returns>
    /// An EventMessage containing given <paramref name="eventMessage"/> as payload, or <paramref name="eventMessage"/>
    /// if it already implements EventMessage.
    /// </returns>
    public static IEventMessage<TMessage> AsEventMessage<TMessage>(
        object eventMessage,
        Func<DateTime>? timestampSupplier = null)
        where TMessage : class
    {
        if (eventMessage is IEventMessage<TMessage> typedEventMessage)
        {
            return typedEventMessage;
        }

        if (eventMessage is IMessage<TMessage> message)
        {
            return new GenericEventMessage<TMessage>(message, timestampSupplier ?? SystemTimestampSupplier);
        }

        return new GenericEventMessage<TMessage>(
            new GenericMessage<TMessage>((TMessage)eventMessage),
            timestampSupplier ?? SystemTimestampSupplier);
    }
}
