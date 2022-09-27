namespace Axon;

using Axon.Messaging;

/// <summary>
/// The commandBus extensions.
/// </summary>
public static class CommandBusExtensions
{
    /// <summary>
    /// Subscribe the given <paramref name="handler"/> to commands with given <paramref name="commandName"/>.
    /// </summary>
    /// <param name="commandBus">The command bus.</param>
    /// <param name="commandName">The name of the command to subscribe the handler to.</param>
    /// <param name="handler">The handler that handles the given type of command.</param>
    /// <returns>>A <see cref="Task"/> that represents the asynchronous subscribe operation.</returns>
    public static Task<IRegistration> SubscribeAsync(
        this ICommandBus commandBus,
        string commandName,
        MessageHandlerCallback<ICommandMessage<object>> handler)
    {
        var wrappedHandler = new WrappedMessageHandlerCallback<ICommandMessage<object>>(handler);
        return commandBus.SubscribeAsync(commandName, wrappedHandler);
    }
}
