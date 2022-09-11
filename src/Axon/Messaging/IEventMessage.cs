namespace Axon.Messaging;

/// <summary>
/// Represents a Message wrapping an Event, which is represented by its payload. An Event is a representation of an
/// occurrence of an event (i.e. anything that happened any might be of importance to any other component) in the
/// application. It contains the data relevant for components that need to act based on that event.
/// </summary>
/// <typeparam name="TPayload">The type of payload contained in this Message.</typeparam>
public interface IEventMessage<out TPayload> : IMessage<TPayload>
    where TPayload : class
{
    /// <summary>
    /// Gets the timestamp of this event. The timestamp is set to the date and time the event was reported.
    /// </summary>
    DateTime Timestamp { get; }
}
