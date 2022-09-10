namespace Axon;

using System.Collections.Concurrent;

/// <summary>
/// Implementation of the <see cref="IEventBus"/> that dispatches events in the CallContext that publishes them.
/// </summary>
public class SimpleEventBus : IEventBus
{
    private readonly ConcurrentDictionary<int, Func<List<object>, Task>> eventProcessors = new();

    /// <inheritdoc />
    public Task<IAsyncDisposable> SubscribeAsync(Func<List<object>, Task> messageProcessor)
    {
        // TODO: Log if subscriber is already added.
        _ = this.eventProcessors.TryAdd(messageProcessor.GetHashCode(), messageProcessor);

        return Task.FromResult((IAsyncDisposable)new Registration(() =>
            this.eventProcessors.Remove(messageProcessor.GetHashCode(), out _)));
    }

    /// <inheritdoc />
    public Task PublishAsync(params object[] events) => this.PublishAsync(events.ToList());

    /// <inheritdoc />
    public Task PublishAsync(List<object> events) => this.ProcessEventsAsync(events);

    /// <summary>
    /// Process given events.
    /// </summary>
    /// <param name="events">Events published to this Event Bus.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected virtual async Task ProcessEventsAsync(List<object> events)
    {
        foreach (var eventProcessor in this.eventProcessors.Values.ToList())
        {
            await eventProcessor(events).ConfigureAwait(true);
        }
    }

    // TODO: Extract to IRegistration
    private class Registration : IAsyncDisposable
    {
        private readonly Action unsubscribeAction;

        public Registration(Action unsubscribeAction) => this.unsubscribeAction = unsubscribeAction;

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            this.unsubscribeAction.Invoke();
            return ValueTask.CompletedTask;
        }
    }
}
