namespace Axon;

using Moq;

public class SimpleCommandBusTests
{
    [Fact]
    public async Task
        SendAsync_Should_ThrowNoHandlerForCommandException_When_CommandDispatched_ForNonSubscribedHandler()
    {
        var commandBus = new SimpleCommandBus();
        var command = string.Empty;
        var exception = await Assert.ThrowsAsync<NoHandlerForCommandException>(() => commandBus.SendAsync(command))
            .ConfigureAwait(false);
        Assert.Contains(command.GetType().Name, exception.Message);
    }

    [Fact]
    public async Task SendAsync_Should_InvokeCommandHandler_WhenCommandDispatched()
    {
        // Arrange
        var commandBus = new SimpleCommandBus();

        var commandHandlerMock = new Mock<MessageHandler<object>>();
        var commandHandler = commandHandlerMock.Object;
        var command = string.Empty;
        await commandBus.SubscribeAsync(command.GetType().FullName!, commandHandler).ConfigureAwait(true);

        // Act
        await commandBus.SendAsync(command).ConfigureAwait(true);

        // Assert
        commandHandlerMock.Verify(_ => _.HandleAsync(command), Times.Once);
    }

    [Fact]
    public async Task SendAsync_Should_ReturnCommandResult_WhenCommandDispatched()
    {
        // Arrange
        var commandBus = new SimpleCommandBus();
        var command = new Ping();
        await commandBus.SubscribeAsync(command.GetType().FullName!, new PingCommandHandler()).ConfigureAwait(true);

        // Act
        var result = await commandBus.SendAsync<Pong>(command).ConfigureAwait(true);

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
