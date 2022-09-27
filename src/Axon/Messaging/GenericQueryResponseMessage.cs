namespace Axon.Messaging;

/// <summary>
/// Non-generic static members for GenericQueryResponseMessage.
/// </summary>
public static class GenericQueryResponseMessage
{
    /// <summary>
    /// Creates a <see cref="IQueryResponseMessage{TResult}"/> for the given <paramref name="result"/>. If result
    /// already implements <see cref="IQueryResponseMessage{TResult}"/>, it is returned directly. Otherwise a new
    /// instance of <see cref="IQueryResponseMessage{TResult}"/> is created with the result as payload.
    /// </summary>
    /// <param name="result">The result of a Query, to be wrapped in a QueryResponseMessage.</param>
    /// <typeparam name="TResult">The type of the expected response.</typeparam>
    /// <returns>A <see cref="IQueryResponseMessage{TResult}"/> for the given <paramref name="result"/>, or the result
    /// itself, if already a <see cref="IQueryResponseMessage{TResult}"/>.</returns>
    public static IQueryResponseMessage<TResult> AsResponseMessage<TResult>(object result)
        where TResult : class => AsNullableResponseMessage<TResult>(typeof(TResult), result);

    /// <summary>
    /// Creates a <see cref="IQueryResponseMessage{TResult}"/> for the given <paramref name="result"/> with a
    /// <paramref name="result"/> as the result type. Providing both the result type and the result allows the creation
    /// of a nullable response message, as the implementation does not have to check the type itself, which could result
    /// in payload type being <c>Nullable</c> of <see cref="object"/>.
    /// <para/>
    /// If result already implements <see cref="IQueryResponseMessage{TResult}"/>, it is returned directly. Otherwise a
    /// new <see cref="IQueryResponseMessage{TResult}"/> is created with the declared type as the result type and the
    /// result as payload.
    /// </summary>
    /// <param name="declaredType">The declared type of the Query Response Message to be created.</param>
    /// <param name="result">The result of a Query, to be wrapped in a QueryResponseMessage.</param>
    /// <typeparam name="TResult">The type of response expected.</typeparam>
    /// <returns>A <see cref="IQueryResponseMessage{TResult}"/> for the given <paramref name="result"/>, or the result
    /// itself, if already a <see cref="IQueryResponseMessage{TResult}"/>.</returns>
    public static IQueryResponseMessage<TResult> AsNullableResponseMessage<TResult>(Type declaredType, object? result)
        where TResult : class
    {
        if (result is IQueryResponseMessage<TResult> queryResponseMessage)
        {
            return queryResponseMessage;
        }

        if (result is IResultMessage<TResult> resultMessage)
        {
            if (resultMessage.Exception != null)
            {
                return new GenericQueryResponseMessage<TResult>(
                    declaredType,
                    resultMessage.Exception,
                    resultMessage.MetaData);
            }

            return new GenericQueryResponseMessage<TResult>(
                declaredType,
                resultMessage.Payload,
                resultMessage.MetaData);
        }

        if (result is IMessage<TResult> message)
        {
            if (message.Payload is null)
            {
                return new GenericQueryResponseMessage<TResult>(
                    declaredType,
                    message.Payload,
                    message.MetaData);
            }

            return new GenericQueryResponseMessage<TResult>(
                declaredType,
                message.Payload,
                message.MetaData);
        }

        return new GenericQueryResponseMessage<TResult>(declaredType, (TResult?)result);
    }

    /// <summary>
    /// Creates a Query Response Message with given <paramref name="declaredType"/> and <paramref name="exception"/>.
    /// </summary>
    /// <param name="declaredType">The declared type of the Query Response Message to be created.</param>
    /// <param name="exception">The Exception describing the cause of an error.</param>
    /// <typeparam name="TResult">The type of the payload.</typeparam>
    /// <returns>A message containing exception result.</returns>
    public static IQueryResponseMessage<TResult> AsResponseMessage<TResult>(Type declaredType, Exception exception)
        where TResult : class =>
            new GenericQueryResponseMessage<TResult>(declaredType, exception);
}
