namespace Axon.Messaging;

/// <summary>
/// Message that represents a result of handling some form of request message.
/// </summary>
/// <typeparam name="TResult">The type of payload contained in this Message.</typeparam>
public interface IResultMessage<out TResult> : IMessage<TResult>
    where TResult : class
{
    /// <summary>
    /// Gets a value indicating whether the ResultMessage represents unsuccessful execution.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the Exception in case of exception result message or null in case of successful.
    /// </summary>
    Exception? Exception { get; }
}
