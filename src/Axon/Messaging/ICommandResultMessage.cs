namespace Axon.Messaging;

/// <summary>
/// Message that represents a result from handling a <see cref="ICommandMessage{TPayload}"/>.
/// </summary>
/// <typeparam name="TResult">The type of payload contained in this Message.</typeparam>
public interface ICommandResultMessage<out TResult> : IResultMessage<TResult>
    where TResult : class
{
}
