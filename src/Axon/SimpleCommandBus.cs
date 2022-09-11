namespace Axon;

using System.Collections.Concurrent;

/// <summary>
/// Implementation of the CommandBus that dispatches commands to the handlers subscribed to that specific command's name.
/// </summary>
public class SimpleCommandBus : ICommandBus
{
    private readonly ConcurrentDictionary<string, MessageHandler<object>> subscriptions = new();
    private readonly IDuplicateCommandHandlerResolver duplicateCommandHandlerResolver;

    // TODO: TransactionManager
    // TODO: MessageMonitor
    // TODO: MessageHandlerInterceptors
    // TODO: DefaultCommandCallback
    // TODO: Builder

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleCommandBus"/> class.
    /// </summary>
    /// <param name="duplicateCommandHandlerResolver"><see cref="IDuplicateCommandHandlerResolver"/> used to resolves
    /// the road to take when a duplicate command handler is subscribed.</param>
    public SimpleCommandBus(IDuplicateCommandHandlerResolver duplicateCommandHandlerResolver) =>
        this.duplicateCommandHandlerResolver = duplicateCommandHandlerResolver;

    /// <inheritdoc />
    public Task<TResult> DispatchAsync<TResult>(object command)
    {
        var handler = this.FindCommandHandlerFor(command.GetType().FullName!) ??
                      throw new NoHandlerForCommandException(command);

        return this.HandleAsync<object, TResult>(command, handler);
    }

    /// <inheritdoc />
    public Task DispatchAsync(object command) => this.DispatchAsync<object>(command);

    /// <inheritdoc />
    public Task<IAsyncDisposable> SubscribeAsync<TCommand>(string commandName, MessageHandler<TCommand> handler)
        where TCommand : class
    {
        var commandHandler = (MessageHandler<object>)(object)handler;
        _ = this.subscriptions.AddOrUpdate(
            commandName,
            _ => commandHandler,
            (_, existingHandler) =>
                this.duplicateCommandHandlerResolver.Resolve(commandName, existingHandler, commandHandler));

        return Task.FromResult(
            (IAsyncDisposable)new Registration(() => this.subscriptions.Remove(commandName, out _)));
    }

    /// <summary>
    /// Invoke the actual handler.
    /// </summary>
    /// <param name="command">The actual command to handle.</param>
    /// <param name="handler">The handler that must be invoked for this command.</param>
    /// <typeparam name="TCommand">The type of payload of the command.</typeparam>
    /// <typeparam name="TResult">The type of result expected from the command handler.</typeparam>
    /// <returns>The result of the message handling.</returns>
    protected virtual async Task<TResult> HandleAsync<TCommand, TResult>(
        TCommand command,
        MessageHandler<object> handler)
        where TCommand : class => (TResult)await handler.HandleAsync(command).ConfigureAwait(false);

    private MessageHandler<object>? FindCommandHandlerFor(string commandName) =>
        this.subscriptions.GetValueOrDefault(commandName);

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
