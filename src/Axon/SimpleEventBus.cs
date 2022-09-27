namespace Axon;

using System.Collections.Immutable;
using Axon.Messaging;

/// <summary>
/// Implementation of the <see cref="IEventBus"/> that dispatches events in the CallContext that publishes them.
/// </summary>
public class SimpleEventBus : IEventBus
{
    private readonly ConcurrentHashSet<MessageProcessor<IEventMessage<object>>> eventProcessors = new();

    /// <inheritdoc />
    public Task<IRegistration> SubscribeAsync(MessageProcessor<IEventMessage<object>> messageProcessor)
    {
        // TODO: Log if subscriber is already added.
        _ = this.eventProcessors.Add(messageProcessor);

        return Task.FromResult<IRegistration>(new Registration(() =>
            this.eventProcessors.TryRemove(messageProcessor)));
    }

    /// <inheritdoc />
    public Task PublishAsync(params IEventMessage<object>[] events) => this.PublishAsync(events.ToList());

    /// <inheritdoc />
    public Task PublishAsync(List<IEventMessage<object>> events) => this.ProcessEventsAsync(events);

    /// <summary>
    /// Process given events.
    /// </summary>
    /// <param name="events">Events published to this Event Bus.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected virtual async Task ProcessEventsAsync(List<IEventMessage<object>> events)
    {
        foreach (var eventProcessor in this.eventProcessors.ToImmutableHashSet())
        {
            await eventProcessor(events).ConfigureAwait(true);
        }
    }
}
