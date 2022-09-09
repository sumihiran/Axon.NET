namespace Axon;

using System.Collections.Concurrent;

/// <summary>
/// Implementation of the CommandBus that dispatches commands to the handlers subscribed to that specific command's name.
/// </summary>
public class SimpleCommandBus
{
    private readonly ConcurrentDictionary<string, MessageHandler<object>> subscriptions = new();

    /// <summary>
    /// Asynchronously send the given <paramref name="command"/> to a single handler the CommandHandler subscribed to
    /// the given <paramref name="command"/>'s name.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <typeparam name="TResult">The type of the expected result.</typeparam>
    /// <returns>A task that represents the send operation. The task result contains the handler response.</returns>
    /// <exception cref="NoHandlerForCommandException">Thrown when no command handler is registered
    /// for the given <paramref name="command"/>.
    /// </exception>
    public Task<TResult> SendAsync<TResult>(object command)
    {
        var handler = this.FindCommandHandlerFor(command.GetType().FullName!) ??
                      throw new NoHandlerForCommandException(command);

        return this.HandleAsync<object, TResult>(command, handler);
    }

    /// <summary>
    /// Asynchronously send the given <paramref name="command"/> to a single handler the CommandHandler subscribed to
    /// the given <paramref name="command"/>'s name via dynamic dispatch.
    /// <paramref name="command"/>'s name.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    public Task SendAsync(object command) => this.SendAsync<object>(command);

    /// <summary>
    /// Subscribe the given <paramref name="handler"/> to commands with given <paramref name="commandName"/>.
    /// </summary>
    /// <param name="commandName">The name of the command to subscribe the handler to.</param>
    /// <param name="handler">The handler instance that handles the given type of command.</param>
    /// <typeparam name="TCommand">The type of payload of the command.</typeparam>
    /// <returns>>A task that represents the asynchronous subscribe operation.</returns>
    public Task SubscribeAsync<TCommand>(string commandName, MessageHandler<TCommand> handler)
        where TCommand : class
    {
        // TODO: Check for duplicates
        // TODO: Return registration
        _ = this.subscriptions.GetOrAdd(commandName, (MessageHandler<object>)(object)handler);
        return Task.CompletedTask;
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
}
