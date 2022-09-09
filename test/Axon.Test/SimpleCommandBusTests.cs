namespace Axon;

using Moq;

public class SimpleCommandBusTests
{
    private static readonly Mock<IDuplicateCommandHandlerResolver> DuplicateCommandHandlerResolverMock = new();
    private readonly ICommandBus commandBus = new SimpleCommandBus(DuplicateCommandHandlerResolverMock.Object);

    [Fact]
    public async Task Should_InvokeDuplicateCommandHandlerResolver_When_DuplicateCommandHandlerSubscribed()
    {
        // Arrange
        var command = new object();
        var commandName = command.GetType().FullName!;
        var initialHandlerMock = new Mock<MessageHandler<object>>();
        var duplicateHandlerMock = new Mock<MessageHandler<object>>();
        DuplicateCommandHandlerResolverMock
            .Setup(r => r.Resolve(commandName, initialHandlerMock.Object, duplicateHandlerMock.Object))
            .Returns(initialHandlerMock.Object);

        // Act
        // Subscribe the initial handler
        await this.commandBus.SubscribeAsync(commandName, initialHandlerMock.Object).ConfigureAwait(false);

        // Then, subscribe a duplicate
        await this.commandBus.SubscribeAsync(commandName, duplicateHandlerMock.Object).ConfigureAwait(false);

        // And after sending a test command, it should be handled by the initial handler
        await this.commandBus.SendAsync(command).ConfigureAwait(false);

        // Assert
        initialHandlerMock.Verify(_ => _.HandleAsync(command), Times.Once);
        DuplicateCommandHandlerResolverMock.Verify(
            _ => _.Resolve(commandName, initialHandlerMock.Object, duplicateHandlerMock.Object), Times.Once);
    }

    [Fact]
    public async Task Should_InvokeExpectedHandler_When_DuplicateCommandHandlerSubscribed()
    {
        // Arrange
        var command = new object();
        var commandName = command.GetType().FullName!;
        var initialHandlerMock = new Mock<MessageHandler<object>>();
        var duplicateHandlerMock = new Mock<MessageHandler<object>>();
        var expectedHandlerMock = new Mock<MessageHandler<object>>();

        DuplicateCommandHandlerResolverMock
            .Setup(r => r.Resolve(commandName, initialHandlerMock.Object, duplicateHandlerMock.Object))
            .Returns(expectedHandlerMock.Object);

        // Act
        await this.commandBus.SubscribeAsync(commandName, initialHandlerMock.Object).ConfigureAwait(false);
        await this.commandBus.SubscribeAsync(commandName, duplicateHandlerMock.Object).ConfigureAwait(false);

        // And after sending a test command, it should be handled by the expected handler
        await this.commandBus.SendAsync(command).ConfigureAwait(false);

        // Assert
        initialHandlerMock.Verify(_ => _.HandleAsync(command), Times.Never);
        duplicateHandlerMock.Verify(_ => _.HandleAsync(command), Times.Never);
        expectedHandlerMock.Verify(_ => _.HandleAsync(command), Times.Once);
    }

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
