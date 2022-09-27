namespace Axon;

using Axon.Messaging;

/// <summary>
/// Delegate describing a callback that is invoked when command handler execution has finished.
/// </summary>
/// <param name="commandMessage">The <see cref="ICommandMessage{TPayload}"/> that was dispatched.</param>
/// <param name="commandResultMessage">
/// The <see cref="ICommandResultMessage{TResult}"/> of the command handling execution.
/// </param>
/// <typeparam name="TPayload">The type of payload of the command.</typeparam>
/// <typeparam name="TResult">The type of result of the command handling.</typeparam>
/// <returns>
/// A <see cref="Task"/> that represents completion of <see cref="CommandCallback{TPayload,TResult}"/> execution.
/// </returns>
public delegate Task CommandCallback<in TPayload, in TResult>(
    ICommandMessage<TPayload> commandMessage, ICommandResultMessage<TResult> commandResultMessage)
    where TPayload : class
    where TResult : class;
