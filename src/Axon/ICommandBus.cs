namespace Axon;

/// <summary>
/// The mechanism that dispatches Command objects to their appropriate CommandHandler. CommandHandlers can subscribe and
/// unsubscribe to specific command on the command bus.
/// Only a single handler may be subscribed for a single command name at any time.
/// </summary>
public interface ICommandBus
{
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
    Task<TResult> SendAsync<TResult>(object command);

    /// <summary>
    /// Asynchronously send the given <paramref name="command"/> to a single handler the CommandHandler subscribed to
    /// the given <paramref name="command"/>'s name via dynamic dispatch.
    /// <paramref name="command"/>'s name.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task SendAsync(object command);

    /// <summary>
    /// Subscribe the given <paramref name="handler"/> to commands with given <paramref name="commandName"/>.
    /// </summary>
    /// <param name="commandName">The name of the command to subscribe the handler to.</param>
    /// <param name="handler">The handler instance that handles the given type of command.</param>
    /// <typeparam name="TCommand">The type of payload of the command.</typeparam>
    /// <returns>>A task that represents the asynchronous subscribe operation.</returns>
    Task<IAsyncDisposable> SubscribeAsync<TCommand>(string commandName, MessageHandler<TCommand> handler)
        where TCommand : class;
}
