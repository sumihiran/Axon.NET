namespace Axon;

using Axon.Messaging;

/// <summary>
/// Represents a wrapped <see cref="DuplicateCommandHandlerResolver"/>.
/// </summary>
internal class WrappedDuplicateCommandHandlerCallbackResolver : IDuplicateCommandHandlerResolver
{
    private readonly DuplicateCommandHandlerResolver duplicateResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="WrappedDuplicateCommandHandlerCallbackResolver"/> class.
    /// </summary>
    /// <param name="duplicateResolver">
    /// The <see cref="DuplicateCommandHandlerResolver"/> used to resolves the road to take when a duplicate command
    /// handler is subscribed.
    /// </param>
    public WrappedDuplicateCommandHandlerCallbackResolver(
        DuplicateCommandHandlerResolver duplicateResolver) => this.duplicateResolver = duplicateResolver;

    /// <inheritdoc />
    public IMessageHandler<ICommandMessage<object>> Resolve(
        string commandName,
        IMessageHandler<ICommandMessage<object>> registeredHandler,
        IMessageHandler<ICommandMessage<object>> candidateHandler) =>
        this.duplicateResolver(commandName, registeredHandler, candidateHandler);
}
