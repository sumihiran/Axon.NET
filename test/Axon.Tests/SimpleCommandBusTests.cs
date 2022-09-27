namespace Axon;

using Axon.Messaging;
using Moq;

public class SimpleCommandBusTests
{
    // Arrange
    private static readonly ICommandMessage<object> Command =
        GenericCommandMessage.AsCommandMessage<object>(new object());

    private static readonly Mock<IMessageHandler<ICommandMessage<object>>> CommandHandlerMock =
        CreateCommandMessageHandlerMock();

    private static readonly Mock<IDuplicateCommandHandlerResolver> DuplicateCommandHandlerResolverMock = new();

    private readonly SimpleCommandBus commandBus = new SimpleCommandBus.Builder()
        .WithDuplicateCommandHandlerResolver(DuplicateCommandHandlerResolverMock.Object)
        .Build();

    private static IMessageHandler<ICommandMessage<object>> CommandHandler => CommandHandlerMock.Object;

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
        await registration.CancelAsync();

        var exception = await Assert
            .ThrowsAsync<NoHandlerForCommandException>(() => this.commandBus.DispatchAsync(Command));

        // Assert
        CommandHandlerMock.Verify(_ => _.HandleAsync(Command), Times.Once);
        Assert.Contains(CommandName, exception.Message);
    }

    [Fact]
    public async Task Should_ThrowNoHandlerForCommandException_When_UnsupportedHandler()
    {
        // Arrange
        CommandHandlerMock.Invocations.Clear();

        var unsupportedHandlerMock = new Mock<IMessageHandler<ICommandMessage<object>>>();
        unsupportedHandlerMock.Setup(_ => _.CanHandle(Command)).Returns(false);

        // Act
        await using var registration = await this.commandBus.SubscribeAsync(CommandName, unsupportedHandlerMock.Object);
        var exception = await Assert
            .ThrowsAsync<NoHandlerForCommandException>(() => this.commandBus.DispatchAsync(Command));

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

        // Act
        await using var registration =
            await this.commandBus.SubscribeAsync(command.CommandName, _ => Task.FromResult((object?)expectedResult));
        var result = await this.commandBus.DispatchAsync<Pong>(command);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResult, result.Payload);
    }

    [Fact]
    public async Task DispatchAsync_FireAndForget_Should_InvokeDefaultCallback()
    {
        // Arrange
        var expectedResultPayload = new Pong();
        ICommandMessage<object>? commandMessage = null;
        ICommandResultMessage<object>? commandResultMessage = null;

        var sut = new SimpleCommandBus.Builder()
            .WithDefaultCommandCallback((message, resultMessage) =>
            {
                commandMessage = message;
                commandResultMessage = resultMessage;
                return Task.CompletedTask;
            })
            .Build();

        Task<object?> Handler(ICommandMessage<object> command) => Task.FromResult<object?>(expectedResultPayload);

        var command = GenericCommandMessage.AsCommandMessage(new Ping());

        // Act
        await using var registration = await sut.SubscribeAsync(command.CommandName, Handler);

        await sut.DispatchAsync(command);

        // Assert
        Assert.NotNull(commandMessage);
        Assert.NotNull(commandResultMessage);
        Assert.Equal(command.Payload, commandMessage.Payload);
        Assert.True(commandResultMessage.IsSuccess);
        Assert.Equal(expectedResultPayload, commandResultMessage.Payload);
    }

    [Fact]
    public async Task Should_ReturnResultWithException_When_HandlerThrowsException()
    {
        // Arrange
        var sut = this.commandBus;
        var expectedException = new ErrorException("fail");
        Task<object?> FailHandler(ICommandMessage<object> commandMessage) => throw expectedException;

        // Act
        await using var registration = await sut.SubscribeAsync(CommandName, FailHandler);
        var result = await sut.DispatchAsync<object>(Command);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedException, result.Exception);
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
        await this.commandBus.SubscribeAsync(CommandName, initialHandlerMock.Object);

        // Then, subscribe a duplicate
        await this.commandBus.SubscribeAsync(CommandName, duplicateHandlerMock.Object);

        // And after sending a test command, it should be handled by the initial handler
        await this.commandBus.DispatchAsync(Command);

        // Assert
        initialHandlerMock.Verify(_ => _.HandleAsync(Command), Times.Once);
        DuplicateCommandHandlerResolverMock.Verify(
            _ => _.Resolve(CommandName, initialHandlerMock.Object, duplicateHandlerMock.Object), Times.Once);
    }

    [Fact]
    public async Task Should_InvokeDuplicateCommandHandlerResolverCallback_When_DuplicateCommandHandlerSubscribed()
    {
        // Arrange
        var invocations = 0;

        IMessageHandler<ICommandMessage<object>> DuplicateHandlerCallback(
            string s,
            IMessageHandler<ICommandMessage<object>> registeredHandler,
            IMessageHandler<ICommandMessage<object>> messageHandler)
        {
            invocations++;
            return registeredHandler;
        }

        var sut = new SimpleCommandBus.Builder().WithDuplicateCommandHandlerResolver(DuplicateHandlerCallback).Build();

        var initialHandlerMock = new Mock<MessageHandlerCallback<ICommandMessage<object>>>();
        var duplicateHandlerMock = new Mock<MessageHandlerCallback<ICommandMessage<object>>>();

        // Act
        // Subscribe the initial handler
        await sut.SubscribeAsync(CommandName, initialHandlerMock.Object);

        // Then, subscribe a duplicate
        await sut.SubscribeAsync(CommandName, duplicateHandlerMock.Object);

        // And after sending a test command, it should be handled by the initial handler
        await sut.DispatchAsync(Command);

        // Assert
        Assert.Equal(1, invocations);
        initialHandlerMock.Verify(_ => _(Command), Times.Once);
    }

    [Fact]
    public async Task Should_InvokeExpectedHandler_When_DuplicateCommandHandlerSubscribed()
    {
        // Arrange
        var initialHandlerMock = CreateCommandMessageHandlerMock();
        var duplicateHandlerMock = CreateCommandMessageHandlerMock();
        var expectedHandlerMock = CreateCommandMessageHandlerMock();
        var sut = new SimpleCommandBus.Builder()
            .WithDuplicateCommandHandlerResolver((_, registered, candidateHandler) => expectedHandlerMock.Object)
            .Build();

        // Act
        await sut.SubscribeAsync(CommandName, initialHandlerMock.Object);
        await sut.SubscribeAsync(CommandName, duplicateHandlerMock.Object);

        // And after sending a test command, it should be handled by the expected handler
        await sut.DispatchAsync(Command);

        // Assert
        initialHandlerMock.Verify(_ => _.HandleAsync(Command), Times.Never);
        duplicateHandlerMock.Verify(_ => _.HandleAsync(Command), Times.Never);
        expectedHandlerMock.Verify(_ => _.HandleAsync(Command), Times.Once);
    }

    private static Mock<IMessageHandler<ICommandMessage<object>>> CreateCommandMessageHandlerMock() =>
        new() { CallBase = true };

    private record Ping;

    private class Pong
    {
    }

    private class ErrorException : Exception
    {
        public ErrorException(string? message)
            : base(message)
        {
        }
    }
}
