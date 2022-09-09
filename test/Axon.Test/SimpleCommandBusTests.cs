namespace Axon;

using Moq;

public class SimpleCommandBusTests
{
    private readonly ICommandBus commandBus = new SimpleCommandBus();

    [Fact]
    public async Task
        SendAsync_Given_NoHandlerSubscribed_Should_ThrowNoHandlerForCommandException_When_CommandDispatched()
    {
        var command = new object();
        var exception = await Assert.ThrowsAsync<NoHandlerForCommandException>(() => this.commandBus.SendAsync(command))
            .ConfigureAwait(false);
        Assert.Contains(command.GetType().Name, exception.Message);
    }

    [Fact]
    public async Task Should_ThrowNoHandlerForCommandException_When_HandlerUnsubscribed()
    {
        // Arrange
        var command = new object();

        var commandHandlerMock = new Mock<MessageHandler<object>>();
        var commandHandler = commandHandlerMock.Object;

        // Act
        var registration = await this.commandBus.SubscribeAsync(command.GetType().FullName!, commandHandler)
            .ConfigureAwait(false);
        await this.commandBus.SendAsync(command).ConfigureAwait(false);
        await registration.DisposeAsync().ConfigureAwait(false);
        var exception = await Assert.ThrowsAsync<NoHandlerForCommandException>(() => this.commandBus.SendAsync(command))
            .ConfigureAwait(false);

        // Assert
        commandHandlerMock.Verify(_ => _.HandleAsync(command), Times.Once);
        Assert.Contains(command.GetType().Name, exception.Message);
    }

    [Fact]
    public async Task SendAsync_Should_InvokeCommandHandler_WhenCommandDispatched()
    {
        // Arrange
        var commandHandlerMock = new Mock<MessageHandler<object>>();
        var commandHandler = commandHandlerMock.Object;
        var command = new object();

        // Act
        await this.commandBus.SubscribeAsync(command.GetType().FullName!, commandHandler).ConfigureAwait(true);

        await this.commandBus.SendAsync(command).ConfigureAwait(true);

        // Assert
        commandHandlerMock.Verify(_ => _.HandleAsync(command), Times.Once);
    }

    [Fact]
    public async Task SendAsync_Should_ReturnCommandResult_WhenCommandDispatched()
    {
        // Arrange
        var command = new Ping();

        // Act
        await this.commandBus.SubscribeAsync(command.GetType().FullName!, new PingCommandHandler())
            .ConfigureAwait(true);
        var result = await this.commandBus.SendAsync<Pong>(command).ConfigureAwait(true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new Pong(), result);
    }

    private record Ping;

    private record Pong;

    private class PingCommandHandler : MessageHandler<object>
    {
        /// <inheritdoc />
        public override Task<object> HandleAsync(object message) => Task.FromResult((object)new Pong());
    }
}
