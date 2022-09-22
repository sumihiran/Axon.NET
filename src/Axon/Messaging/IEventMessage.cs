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
    // TODO: override identifier

    /// <inheritdoc />
    TPayload IMessage<TPayload>.Payload => this.Payload;

    /// <summary>
    /// Gets the payload of the event message.
    /// </summary>
    new TPayload Payload { get; }

    /// <summary>
    /// Gets the timestamp of this event. The timestamp is set to the date and time the event was reported.
    /// </summary>
    DateTime Timestamp { get; }

    /// <inheritdoc />
    IMessage<TPayload> IMessage<TPayload>.WithMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        this.WithMetaData(metaData);

    /// <inheritdoc />
    IMessage<TPayload> IMessage<TPayload>.AndMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        this.AndMetaData(metaData);

    /// <summary>
    /// Returns a copy of this EventMessage with the given <paramref name="metaData"/>. The payload,
    /// <see cref="Timestamp"/> and <see cref="IMessage{TPayload}.Identifier"/> remain unchanged.
    /// </summary>
    /// <param name="metaData">The new MetaData for the Message.</param>
    /// <returns>A copy of this message with the given MetaData.</returns>
    new IEventMessage<TPayload> WithMetaData(ICollection<KeyValuePair<string, object>> metaData);

    /// <summary>
    /// Returns a copy of this CommandMessage with it MetaData merged with the given. The payload remain unchanged.
    /// </summary>
    /// <param name="metaData">The MetaData to merge with.</param>
    /// <returns>A copy of this message with the given MetaData.</returns>
    new IEventMessage<TPayload> AndMetaData(ICollection<KeyValuePair<string, object>> metaData);
}
