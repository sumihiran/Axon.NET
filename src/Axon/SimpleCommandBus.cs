namespace Axon;

using System.Collections.Concurrent;
using Axon.Messaging;

/// <summary>
/// Implementation of the CommandBus that dispatches commands to the handlers subscribed to that specific command's name.
/// </summary>
public class SimpleCommandBus : ICommandBus
{
    private readonly ConcurrentDictionary<string, IMessageHandler> subscriptions = new();
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
    public async Task<ICommandResultMessage<TResult>> DispatchAsync<TResult>(ICommandMessage<object> command)
        where TResult : class
    {
        var handler = this.FindCommandHandlerFor(command.CommandName);

        if (handler is null || !handler.CanHandle(command))
        {
            throw new NoHandlerForCommandException(
                $"No handler was subscribed to command {command.CommandName}");
        }

        return GenericCommandResultMessage.AsCommandResultMessage<TResult>(
            await this.HandleAsync<TResult>(command, handler).ConfigureAwait(false));
    }

    /// <inheritdoc />
    public Task DispatchAsync(ICommandMessage<object> command)
        => this.DispatchAsync<Unit>(command);

    /// <inheritdoc />
    public Task<IAsyncDisposable> SubscribeAsync(string commandName, IMessageHandler handler)
    {
        _ = this.subscriptions.AddOrUpdate(
            commandName,
            _ => handler,
            (_, existingHandler) =>
                this.duplicateCommandHandlerResolver.Resolve(commandName, existingHandler, handler));

        return Task.FromResult(
            (IAsyncDisposable)new Registration(() => this.subscriptions.Remove(commandName, out _)));
    }

    /// <summary>
    /// Invoke the actual handler.
    /// </summary>
    /// <param name="command">The actual command to handle.</param>
    /// <param name="handler">The handler that must be invoked for this command.</param>
    /// <typeparam name="TResult">The type of result expected from the command handler.</typeparam>
    /// <returns>The result of the message handling.</returns>
    protected virtual async Task<IResultMessage<TResult>> HandleAsync<TResult>(
        ICommandMessage<object> command,
        IMessageHandler handler)
        where TResult : class
    {
        IResultMessage<TResult> resultMessage;
        try
        {
            var handlingTask = handler.HandleAsync(command);

            if (typeof(TResult) == typeof(Unit))
            {
                // TODO: Keep track of tasks
                handlingTask.Start(TaskScheduler.Default);
                return GenericResultMessage.AsResultMessage<TResult>(Unit.Value);
            }

            var result = await handlingTask.ConfigureAwait(false);
            if (result is IResultMessage<TResult> checkedResultMessage)
            {
                resultMessage = checkedResultMessage;
            }
            else if (result is IMessage<TResult> message)
            {
                resultMessage = new GenericResultMessage<TResult>(message.Payload, message.MetaData);
            }
            else
            {
                resultMessage = new GenericResultMessage<TResult>((TResult?)result);
            }
        }
        catch (Exception exception)
        {
            resultMessage = GenericResultMessage.AsErrorResultMessage<TResult>(exception);
        }

        return resultMessage;
    }

    private IMessageHandler? FindCommandHandlerFor(string commandName) =>
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
