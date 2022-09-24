namespace Axon;

using Axon.Messaging;

/// <summary>
/// Chooses what to do when a duplicate handler is registered, returning the handler that should be selected for
/// command handling, or otherwise throwing an exception to reject registration altogether.
/// </summary>
/// <param name="commandName">The name of the Command for which the duplicate was detected.</param>
/// <param name="registeredHandler">
/// The <see cref="MessageHandler{TMessage}"/> previously registered with the Command Bus.
/// </param>
/// <param name="candidateHandler">
/// The <see cref="MessageHandler{TMessage}"/>  that is newly registered and conflicts with
/// the existing registration.
/// </param>
/// <returns>The resolved <see cref="MessageHandler{TMessage}"/>. Could be the <paramref name="registeredHandler"/>,
/// the <paramref name="candidateHandler"/> or another handler entirely.</returns>
/// <exception cref="InvalidOperationException">When registration operation should fail.</exception>
public delegate MessageHandler<ICommandMessage<object>> DuplicateCommandHandlerResolver(
    string commandName,
    MessageHandler<ICommandMessage<object>> registeredHandler,
    MessageHandler<ICommandMessage<object>> candidateHandler);
