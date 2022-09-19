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
        var pingCommandHandler = new DelegatingCommandHandler<Ping>(_ => Task.FromResult((object?)expectedResult));

        // Act
        await using var registration = await this.commandBus.SubscribeAsync(command.CommandName, pingCommandHandler);
        var result = await this.commandBus.DispatchAsync<Pong>(command);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResult, result.Payload);
    }

    [Fact]
    public async Task Should_ReturnResultWithException_When_HandlerThrowsException()
    {
        // Arrange
        var sut = this.commandBus;
        var expectedException = new ErrorException("fail");
        var failHandler = new DelegatingCommandHandler<object>(_ => throw expectedException);

        // Act
        await using var registration = await sut.SubscribeAsync(CommandName, failHandler);
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

    [Fact]
    public async Task DispatchAsync_FireAndForget_Should_ReturnOnceDispatched()
    {
        // Arrange
        var sut = this.commandBus;
        var cancellationTokenSource = new CancellationTokenSource();
        var initMonitor = new SemaphoreSlim(1, 1);
        var completionTask = Task.Delay(TimeSpan.FromMinutes(1), cancellationTokenSource.Token);
        var commandHandler = new DelegatingCommandHandler<object>(async _ =>
        {
            initMonitor.Release(1);
            completionTask.Start(TaskScheduler.Current);
            await completionTask.WaitAsync(CancellationToken.None);
            return null;
        });

        // Act
        await initMonitor.WaitAsync(CancellationToken.None);
        await using var registration = await sut.SubscribeAsync(CommandName, commandHandler);

        await sut.DispatchAsync(Command);

        // completes when monitor released within the handler
        await initMonitor.WaitAsync(CancellationToken.None);

        // Assert
        Assert.NotEqual(TaskStatus.RanToCompletion, completionTask.Status);
    }

    private static Mock<MessageHandler<ICommandMessage<object>>> CreateCommandMessageHandlerMock() =>
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

    private class DelegatingCommandHandler<TPayload> : MessageHandler<ICommandMessage<TPayload>>
        where TPayload : class
    {
        private readonly Func<ICommandMessage<TPayload>, Task<object?>> handleAction;

        public DelegatingCommandHandler(Func<ICommandMessage<TPayload>, Task<object?>> handleAction) =>
            this.handleAction = handleAction;

        public override Task<object?> HandleAsync(ICommandMessage<TPayload> message) => this.handleAction(message);
    }
}
