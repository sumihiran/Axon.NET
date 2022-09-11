namespace Axon;

using Axon.Messaging;

/// <summary>
/// The mechanism that dispatches Command objects to their appropriate CommandHandler. CommandHandlers can subscribe and
/// unsubscribe to specific command on the command bus.
/// Only a single handler may be subscribed for a single command name at any time.
/// </summary>
public interface ICommandBus
{
    /// <summary>
    /// Asynchronously dispatch the given <paramref name="command"/> to the CommandHandler subscribed the given
    /// <paramref name="command"/>'s name. Once the command is processed, the result is returned.
    /// </summary>
    /// <param name="command">The command to dispatch.</param>
    /// <typeparam name="TResult">The type of the expected result.</typeparam>
    /// <returns>A task that represents the dispatch operation. The task result contains the handler response.</returns>
    /// <exception cref="NoHandlerForCommandException">
    /// Thrown when no command handler is registered for the given <paramref name="command"/>.
    /// </exception>
    Task<TResult> DispatchAsync<TResult>(ICommandMessage<object> command);

    /// <summary>
    /// Asynchronously dispatch the given <paramref name="command"/> to the CommandHandler subscribed the given
    /// <paramref name="command"/>'s name. Implementations may return immediately after asserting a valid handler is
    /// registered for the given command.
    /// </summary>
    /// <param name="command">The command to dispatch.</param>
    /// <typeparam name="TCommand">The payload type of the command to dispatch.</typeparam>
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
    Task DispatchAsync<TCommand>(ICommandMessage<TCommand> command)
        where TCommand : class;

    /// <summary>
    /// Subscribe the given <paramref name="handler"/> to commands with given <paramref name="commandName"/>.
    /// </summary>
    /// <param name="commandName">The name of the command to subscribe the handler to.</param>
    /// <param name="handler">The handler instance that handles the given type of command.</param>
    /// <typeparam name="TCommand">The type of payload of the command.</typeparam>
    /// <returns>>A task that represents the asynchronous subscribe operation.</returns>
    Task<IAsyncDisposable> SubscribeAsync<TCommand>(string commandName, MessageHandler<TCommand> handler)
        where TCommand : ICommandMessage<object>;
}
