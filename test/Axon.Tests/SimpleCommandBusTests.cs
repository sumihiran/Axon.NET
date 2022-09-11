namespace Axon;

using Axon.Messaging;
using Moq;

public class SimpleCommandBusTests
{
    // Arrange
    private static readonly ICommandMessage<object> Command =
        GenericCommandMessage.AsCommandMessage<object>(new object());

    private static readonly Mock<MessageHandler<ICommandMessage<object>>> CommandHandlerMock = new();

    private static readonly Mock<IDuplicateCommandHandlerResolver> DuplicateCommandHandlerResolverMock = new();
    private readonly ICommandBus commandBus = new SimpleCommandBus(DuplicateCommandHandlerResolverMock.Object);

    private static MessageHandler<ICommandMessage<object>> CommandHandler => CommandHandlerMock.Object;

    private static string CommandName => Command.CommandName;

    [Fact]
    public async Task
        DispatchAsync_Given_NoHandlerSubscribed_When_CommandDispatched_Then_ThrowNoHandlerForCommandException()
    {
        var exception = await Assert
            .ThrowsAsync<NoHandlerForCommandException>(() => this.commandBus.DispatchAsync(Command));
        Assert.Contains(CommandName, exception.Message);
    }

    [Fact]
    public async Task Should_ThrowNoHandlerForCommandException_When_HandlerUnsubscribed()
    {
        // Arrange
        CommandHandlerMock.Reset();

        // Act
        var registration = await this.commandBus.SubscribeAsync(CommandName, CommandHandler);
        await this.commandBus.DispatchAsync(Command);
        await registration.DisposeAsync();

        var exception = await Assert
            .ThrowsAsync<NoHandlerForCommandException>(() => this.commandBus.DispatchAsync(Command))
            .ConfigureAwait(false);

        // Assert
        CommandHandlerMock.Verify(_ => _.HandleAsync(Command), Times.Once);
        Assert.Contains(CommandName, exception.Message);
    }

    [Fact]
    public async Task Should_InvokeCommandHandler_WhenCommandDispatched()
    {
        // Arrange
        CommandHandlerMock.Reset();

        // Act
        await this.commandBus.SubscribeAsync(CommandName, CommandHandler);

        await this.commandBus.DispatchAsync(Command);

        // Assert
        CommandHandlerMock.Verify(_ => _.HandleAsync(Command), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnCommandResult_When_CommandDispatched()
    {
        // Arrange
        var command = GenericCommandMessage.AsCommandMessage<Ping>(new Ping());
        var commandName = command.CommandName;
        var pingCommandHandler = new PingCommandHandler();

        // Act
        await this.commandBus.SubscribeAsync(commandName, pingCommandHandler)
            .ConfigureAwait(true);
        var result = await this.commandBus.DispatchAsync<Pong>(command).ConfigureAwait(true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new Pong(), result);
    }

    [Fact]
    public async Task Should_InvokeDuplicateCommandHandlerResolver_When_DuplicateCommandHandlerSubscribed()
    {
        // Arrange
        var initialHandlerMock = new Mock<MessageHandler<ICommandMessage<object>>>();
        var duplicateHandlerMock = new Mock<MessageHandler<ICommandMessage<object>>>();
        DuplicateCommandHandlerResolverMock
            .Setup(r => r.Resolve(CommandName, initialHandlerMock.Object, duplicateHandlerMock.Object))
            .Returns(initialHandlerMock.Object);

        // Act
        // Subscribe the initial handler
        await this.commandBus.SubscribeAsync(CommandName, initialHandlerMock.Object).ConfigureAwait(false);

        // Then, subscribe a duplicate
        await this.commandBus.SubscribeAsync(CommandName, duplicateHandlerMock.Object).ConfigureAwait(false);

        // And after sending a test command, it should be handled by the initial handler
        await this.commandBus.DispatchAsync(Command).ConfigureAwait(false);

        // Assert
        initialHandlerMock.Verify(_ => _.HandleAsync(Command), Times.Once);
        DuplicateCommandHandlerResolverMock.Verify(
            _ => _.Resolve(CommandName, initialHandlerMock.Object, duplicateHandlerMock.Object), Times.Once);
    }

    [Fact]
    public async Task Should_InvokeExpectedHandler_When_DuplicateCommandHandlerSubscribed()
    {
        // Arrange
        var initialHandlerMock = new Mock<MessageHandler<ICommandMessage<object>>>();
        var duplicateHandlerMock = new Mock<MessageHandler<ICommandMessage<object>>>();
        var expectedHandlerMock = new Mock<MessageHandler<ICommandMessage<object>>>();

        DuplicateCommandHandlerResolverMock
            .Setup(r => r.Resolve(CommandName, initialHandlerMock.Object, duplicateHandlerMock.Object))
            .Returns(expectedHandlerMock.Object);

        // Act
        await this.commandBus.SubscribeAsync(CommandName, initialHandlerMock.Object).ConfigureAwait(false);
        await this.commandBus.SubscribeAsync(CommandName, duplicateHandlerMock.Object).ConfigureAwait(false);

        // And after sending a test command, it should be handled by the expected handler
        await this.commandBus.DispatchAsync(Command).ConfigureAwait(false);

        // Assert
        initialHandlerMock.Verify(_ => _.HandleAsync(Command), Times.Never);
        duplicateHandlerMock.Verify(_ => _.HandleAsync(Command), Times.Never);
        expectedHandlerMock.Verify(_ => _.HandleAsync(Command), Times.Once);
    }

    private record Ping;

    private record Pong;

    private class PingCommandHandler : MessageHandler<ICommandMessage<object>>
    {
        /// <inheritdoc />
        public override Task<object> HandleAsync(ICommandMessage<object> message) =>
            Task.FromResult((object)new Pong());
    }
}
