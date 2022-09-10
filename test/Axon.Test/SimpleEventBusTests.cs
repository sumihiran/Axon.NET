namespace Axon;

using Moq;

public class SimpleEventBusTests
{
    private static readonly object EventMessage = new();
    private static readonly List<object> Events = new() { EventMessage };

    private readonly IEventBus eventBus = new SimpleEventBus();
    private readonly Mock<Func<List<object>, Task>> listener1 = new();
    private readonly Mock<Func<List<object>, Task>> listener2 = new();
    private readonly Mock<Func<List<object>, Task>> listener3 = new();

    [Fact]
    public async Task EventIsDispatchedToSubscribedListeners()
    {
        // Act
        await this.eventBus.PublishAsync(Events);

        var subscription1 = await this.eventBus.SubscribeAsync(this.listener1.Object);

        await this.eventBus.PublishAsync(EventMessage);

        var subscription2 = await this.eventBus.SubscribeAsync(this.listener2.Object);
        var subscription3 = await this.eventBus.SubscribeAsync(this.listener3.Object);

        await this.eventBus.PublishAsync(Events);

        await subscription1.DisposeAsync();

        await this.eventBus.PublishAsync(Events);

        await subscription2.DisposeAsync();
        await subscription3.DisposeAsync();

        await this.eventBus.PublishAsync(Events);

        // Assert
        this.listener1.Verify(_ => _.Invoke(Events), Times.Exactly(2));
        this.listener2.Verify(_ => _.Invoke(Events), Times.Exactly(2));
        this.listener3.Verify(_ => _.Invoke(Events), Times.Exactly(2));
    }

    [Fact]
    public async Task Given_DuplicateListenerSubscribed_When_AnEventPublished_Then_ListenerInvokedOnce()
    {
        _ = await this.eventBus.SubscribeAsync(this.listener1.Object);

        // subscribing twice should not make a difference
        _ = await this.eventBus.SubscribeAsync(this.listener1.Object);

        await this.eventBus.PublishAsync(EventMessage);

        this.listener1.Verify(_ => _.Invoke(Events), Times.Once);
    }
}
