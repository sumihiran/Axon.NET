namespace Axon;

using Axon.Messaging;

/// <summary>
/// As such it ingests two <see cref="IMessageHandler"/> instances and returns another one as the resolution.
/// </summary>
public interface IDuplicateCommandHandlerResolver
{
    /// <summary>
    /// Chooses what to do when a duplicate handler is registered, returning the handler that should be selected for
    /// command handling, or otherwise throwing an exception to reject registration altogether.
    /// </summary>
    /// <param name="commandName">The name of the Command for which the duplicate was detected.</param>
    /// <param name="registeredHandler">The <see cref="IMessageHandler"/> previously registered with the Command Bus.</param>
    /// <param name="candidateHandler">The <see cref="IMessageHandler"/>  that is newly registered and conflicts with
    /// the existing registration.</param>
    /// <returns>The resolved <see cref="IMessageHandler"/>. Could be the <paramref name="registeredHandler"/>,
    /// the <paramref name="candidateHandler"/> or another handler entirely.</returns>
    /// <exception cref="InvalidOperationException">When registration operation should fail.</exception>
    IMessageHandler Resolve(string commandName, IMessageHandler registeredHandler, IMessageHandler candidateHandler);
}
