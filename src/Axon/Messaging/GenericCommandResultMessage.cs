namespace Axon.Messaging;

/// <summary>
/// Non-generic static members for <see cref="GenericCommandResultMessage{TResult}"/>.
/// </summary>
public class GenericCommandResultMessage
{
    /// <summary>
    /// Returns the given <paramref name="commandResult"/> as a <see cref="ICommandResultMessage{TResult}"/> instance.
    /// If <paramref name="commandResult"/> already implements <see cref="ICommandResultMessage{TResult}"/>, it is
    /// returned as-is. If <paramref name="commandResult"/> implements <see cref="IMessage{TPayload}"/>, payload and
    /// meta data will be used to construct new <see cref="GenericCommandResultMessage{TPayload}"/>. Otherwise, the
    /// given <paramref name="commandResult"/> is wrapped into a  <see cref="GenericCommandResultMessage{TPayload}"/> as
    /// its payload.
    /// </summary>
    /// <param name="commandResult">
    /// The command result to be wrapped as <see cref="ICommandResultMessage{TResult}"/>.
    /// </param>
    /// <typeparam name="TResult">The type of the payload contained in returned Message.</typeparam>
    /// <returns>
    /// A Message containing given <paramref name="commandResult"/> as payload, or <paramref name="commandResult"/> if
    /// already implements <see cref="ICommandResultMessage{TResult}"/>.
    /// </returns>
    public static ICommandResultMessage<TResult> AsCommandResultMessage<TResult>(object? commandResult)
        where TResult : class
    {
        if (commandResult is ICommandResultMessage<TResult> resultMessage)
        {
            return resultMessage;
        }

        if (commandResult is IMessage<TResult> message)
        {
            return new GenericCommandResultMessage<TResult>(message);
        }

        if (commandResult is Exception exception)
        {
            // TODO: Stop TResult being an exception
            return AsCommandResultErrorMessage<TResult, Exception>(exception);
        }

        return new GenericCommandResultMessage<TResult>((TResult?)commandResult);
    }

    /// <summary>
    /// Creates a Command Result Message with the given <see cref="Exception"/> result.
    /// </summary>
    /// <param name="exception">the Exception describing the cause of an error.</param>
    /// <typeparam name="TResult">The type of the payload contained in returned Message.</typeparam>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <returns>A message containing exception result.</returns>
    public static ICommandResultMessage<TResult> AsCommandResultErrorMessage<TResult, TException>(TException exception)
        where TResult : class
        where TException : Exception =>
        new GenericCommandResultMessage<TResult>(exception);
}
