namespace Axon;

using System.Diagnostics.CodeAnalysis;
using Axon.Messaging;

/// <summary>
/// Chooses what to do when a duplicate handler is registered, returning the handler that should be selected for
/// command handling, or otherwise throwing an exception to reject registration altogether.
/// </summary>
/// <param name="commandName">The name of the Command for which the duplicate was detected.</param>
/// <param name="registeredHandler">
/// The <see cref="IMessageHandler{TMessage}"/> previously registered with the Command Bus.
/// </param>
/// <param name="candidateHandler">
/// The <see cref="IMessageHandler{TMessage}"/>  that is newly registered and conflicts with
/// the existing registration.
/// </param>
/// <returns>The resolved <see cref="IMessageHandler{TMessage}"/>. Could be the <paramref name="registeredHandler"/>,
/// the <paramref name="candidateHandler"/> or another handler entirely.</returns>
/// <exception cref="InvalidOperationException">When registration operation should fail.</exception>
public delegate IMessageHandler<ICommandMessage<object>> DuplicateCommandHandlerResolver(
    string commandName,
    IMessageHandler<ICommandMessage<object>> registeredHandler,
    IMessageHandler<ICommandMessage<object>> candidateHandler);

/// <summary>
/// DuplicateCommandHandlerResolver extensions.
/// </summary>
[SuppressMessage("ReSharper", "SA1649:FileNameMustMatchTypeName", Justification = "Delegate extensions")]
internal static class DuplicateCommandHandlerResolverExtensions
{
    /// <summary>
    /// Wraps the <see cref="DuplicateCommandHandlerResolver"/> inside a
    /// <see cref="WrappedDuplicateCommandHandlerCallbackResolver"/> instance.
    /// </summary>
    /// <param name="resolverCallback">The <see cref="DuplicateCommandHandlerResolver"/>.</param>
    /// <returns>A wrapped <see cref="DuplicateCommandHandlerResolver"/>.</returns>
    public static IDuplicateCommandHandlerResolver Wrap(this DuplicateCommandHandlerResolver resolverCallback)
        => new WrappedDuplicateCommandHandlerCallbackResolver(resolverCallback);
}
