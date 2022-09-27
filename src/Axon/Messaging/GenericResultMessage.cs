namespace Axon.Messaging;

/// <summary>
/// Non-generic static members for GenericResultMessage.
/// </summary>
public class GenericResultMessage
{
    /// <summary>
    /// Returns the given <paramref name="result"/> as a <see cref="IResultMessage{TPayload}"/> instance.
    /// If <paramref name="result"/> already implements <see cref="IResultMessage{TPayload}"/> , it is returned as-is.
    /// If <paramref name="result"/> implements <see cref="IMessage{TPayload}"/>,  payload and meta data will be used to
    /// construct new <see cref="GenericResultMessage{TPayload}"/>. Otherwise the given <paramref name="result"/> is
    /// wrapped into  <see cref="GenericResultMessage{TPayload}"/> as its payload.
    /// </summary>
    /// <param name="result">the command result to be wrapped a  <see cref="IResultMessage{TPayload}"/>.</param>
    /// <typeparam name="TPayload">The type of the payload contained in returned Message.</typeparam>
    /// <returns>
    /// A Message containing given<paramref name="result"/> as payload, or <paramref name="result"/> if it is already.
    /// </returns>
    public static IResultMessage<TPayload> AsResultMessage<TPayload>(TPayload? result)
        where TPayload : class => AsResultMessage<TPayload>((object?)result);

    /// <summary>
    /// Returns the given <paramref name="result"/> as a <see cref="IResultMessage{TPayload}"/> instance.
    /// If <paramref name="result"/> already implements <see cref="IResultMessage{TPayload}"/> , it is returned as-is.
    /// If <paramref name="result"/> implements <see cref="IMessage{TPayload}"/>,  payload and meta data will be used to
    /// construct new <see cref="GenericResultMessage{TPayload}"/>. Otherwise the given <paramref name="result"/> is
    /// wrapped into  <see cref="GenericResultMessage{TPayload}"/> as its payload.
    /// </summary>
    /// <param name="result">the command result to be wrapped a  <see cref="IResultMessage{TPayload}"/>.</param>
    /// <typeparam name="TPayload">The type of the payload contained in returned Message.</typeparam>
    /// <returns>
    /// A Message containing given<paramref name="result"/> as payload, or <paramref name="result"/> if it is already.
    /// </returns>
    public static IResultMessage<TPayload> AsResultMessage<TPayload>(object? result)
        where TPayload : class
    {
        if (result is IResultMessage<TPayload> resultMessage)
        {
            return resultMessage;
        }

        if (result is IMessage<TPayload> message)
        {
            return new GenericResultMessage<TPayload>(message);
        }

        if (result is Exception exception)
        {
            // TODO: Stop TPayload being an exception
            return AsErrorResultMessage<TPayload>(exception);
        }

        return new GenericResultMessage<TPayload>((TPayload?)result);
    }

    /// <summary>
    /// Creates a ResultMessage with the given <see cref="Exception"/> result.
    /// </summary>
    /// <param name="exception">the Exception describing the cause of an error.</param>
    /// <typeparam name="TPayload">The type of the payload contained in returned Message.</typeparam>
    /// <returns>A message containing exception result.</returns>
    public static IResultMessage<TPayload> AsErrorResultMessage<TPayload>(Exception exception)
        where TPayload : class =>
        new GenericResultMessage<TPayload>(exception);

    /// <summary>
    /// Extract <see cref="Exception"/> from the provided <paramref name="message"/> if any.
    /// </summary>
    /// <param name="message">A message.</param>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <returns>An <see cref="Exception"/> or <c>null</c>.</returns>
    internal static Exception? FindResultException<TPayload>(IMessage<TPayload> message)
        where TPayload : class
    {
        if (message is IResultMessage<TPayload> { IsSuccess: false } resultMessage)
        {
            return resultMessage.Exception;
        }

        return null;
    }
}
