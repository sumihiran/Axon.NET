namespace Axon;

using Axon.Messaging;

/// <summary>
/// Specification of the mechanism on which the Event Listeners can subscribe for events and event publishers can publish
/// their events. The event bus dispatches events to all subscribed listeners.
/// </summary>
public interface IEventBus : ISubscribableMessageSource<IEventMessage<object>>
{
    /// <summary>
    /// Publish a collection of events on this bus (one, or multiple). The events will be dispatched to all subscribed
    /// listeners.
    ///
    /// Implementations may treat the given <paramref name="events"/> as a single batch and distribute the events
    /// as such to all subscribed EventListeners.
    /// </summary>
    /// <param name="events">The collection of events to publish.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task PublishAsync(params IEventMessage<object>[] events);

    /// <summary>
    /// Publish a collection of events on this bus (one, or multiple). The events will be dispatched to all subscribed
    /// listeners.
    ///
    /// Implementations may treat the given <paramref name="events"/> as a single batch and distribute the events
    /// as such to all subscribed EventListeners.
    /// </summary>
    /// <param name="events">The collection of events to publish.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task PublishAsync(List<IEventMessage<object>> events);
}
