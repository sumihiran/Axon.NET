namespace Axon;

using Axon.Messaging;
using Moq;

public class SimpleCommandBusTests
{
    // Arrange
    private static readonly ICommandMessage<object> Command =
        GenericCommandMessage.AsCommandMessage<object>(new object());

    private static readonly Mock<MessageHandler<ICommandMessage<object>>> CommandHandlerMock =
        CreateCommandMessageHandlerMock();

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
        CommandHandlerMock.Invocations.Clear();

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
    public async Task Should_ThrowNoHandlerForCommandException_When_UnsupportedHandler()
    {
        // Arrange
        CommandHandlerMock.Invocations.Clear();

        var unsupportedHandlerMock = new Mock<MessageHandler<ICommandMessage<object>>>();
        unsupportedHandlerMock.Setup(_ => _.CanHandle(Command)).Returns(false);

        // Act
        await using var registration = await this.commandBus.SubscribeAsync(CommandName, unsupportedHandlerMock.Object);
        var exception = await Assert
            .ThrowsAsync<NoHandlerForCommandException>(() => this.commandBus.DispatchAsync(Command))
            .ConfigureAwait(false);

        // Assert
        CommandHandlerMock.Verify(_ => _.HandleAsync(Command), Times.Never);
        Assert.Contains(CommandName, exception.Message);
    }

    [Fact]
    public async Task Should_InvokeCommandHandler_WhenCommandDispatched()
    {
        // Arrange
        CommandHandlerMock.Invocations.Clear();

        // Act
        await using var registration = await this.commandBus.SubscribeAsync(CommandName, CommandHandler);

        await this.commandBus.DispatchAsync(Command);

        // Assert
        CommandHandlerMock.Verify(_ => _.HandleAsync(Command), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnCommandResult_When_CommandDispatched()
    {
        // Arrange
        var command = GenericCommandMessage.AsCommandMessage(new Ping());
        var expectedResult = new Pong();
        var pingCommandHandler = new PingCommandHandler(expectedResult);

        // Act
        await using var registration = await this.commandBus.SubscribeAsync(command.CommandName, pingCommandHandler);
        var result = await this.commandBus.DispatchAsync<Pong>(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task Should_InvokeDuplicateCommandHandlerResolver_When_DuplicateCommandHandlerSubscribed()
    {
        // Arrange
        DuplicateCommandHandlerResolverMock.Reset();
        var initialHandlerMock = CreateCommandMessageHandlerMock();
        var duplicateHandlerMock = CreateCommandMessageHandlerMock();

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
        DuplicateCommandHandlerResolverMock.Reset();
        var initialHandlerMock = CreateCommandMessageHandlerMock();
        var duplicateHandlerMock = CreateCommandMessageHandlerMock();
        var expectedHandlerMock = CreateCommandMessageHandlerMock();

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

    private static Mock<MessageHandler<ICommandMessage<object>>> CreateCommandMessageHandlerMock() =>
        new() { CallBase = true };

    private record Ping;

    private class Pong
    {
    }

    private class PingCommandHandler : MessageHandler<ICommandMessage<Ping>>
    {
        private readonly Pong response;

        public PingCommandHandler(Pong response) => this.response = response;

        /// <inheritdoc />
        public override Task<object?> HandleAsync(ICommandMessage<Ping> message) =>
            Task.FromResult((object?)this.response);
    }
}
