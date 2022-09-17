namespace Axon.Messaging;

/// <summary>
/// Non-generic static members for GenericCommandMessage.
/// </summary>
public static class GenericCommandMessage
{
    /// <summary>
    /// Returns the given command as a <see cref="ICommandMessage{TPayload}"/>. If <paramref name="command"/> already
    /// implements <see cref="ICommandMessage{TPayload}"/>, it is returned as-is. When the <paramref name="command"/> is
    /// another implementation of <see cref="IMessage{TPayload}"/>, the <see cref="IMessage{TPayload}.Payload"/> and
    /// <see cref="IMessage{TPayload}.MetaData"/> are used as input for a new
    /// <see cref="GenericCommandMessage{TPayload}"/>. Otherwise, the given <paramref name="command"/> is wrapped into a
    /// <see cref="GenericCommandMessage{TPayload}"/> as its payload.
    /// </summary>
    /// <param name="command">The command to wrap as <see cref="ICommandMessage{TPayload}"/>.</param>
    /// <typeparam name="TMessage">The generic type of the expected payload of the resulting object.</typeparam>
    /// <returns>
    /// A <see cref="ICommandMessage{TPayload}"/> containing given <paramref name="command"/> as payload, a
    /// <paramref name="command"/> if it already implements<see cref="ICommandMessage{TPayload}"/>, or a
    /// <see cref="ICommandMessage{TPayload}"/> based on the result of <seealso cref="IMessage{TPayload}.Payload"/> and
    /// <see cref="IMessage{TPayload}.MetaData"/> for other <see cref="IMessage{TPayload}"/> implementations.
    /// </returns>
    public static ICommandMessage<TMessage> AsCommandMessage<TMessage>(TMessage command)
        where TMessage : class
        => AsCommandMessage<TMessage>((object)command);

    /// <summary>
    /// Returns the given command as a <see cref="ICommandMessage{TPayload}"/>. If <paramref name="command"/> already
    /// implements <see cref="ICommandMessage{TPayload}"/>, it is returned as-is. When the <paramref name="command"/> is
    /// another implementation of <see cref="IMessage{TPayload}"/>, the <see cref="IMessage{TPayload}.Payload"/> and
    /// <see cref="IMessage{TPayload}.MetaData"/> are used as input for a new
    /// <see cref="GenericCommandMessage{TPayload}"/>. Otherwise, the given <paramref name="command"/> is wrapped into a
    /// <see cref="GenericCommandMessage{TPayload}"/> as its payload.
    /// </summary>
    /// <param name="command">The command to wrap as <see cref="ICommandMessage{TPayload}"/>.</param>
    /// <typeparam name="TMessage">The generic type of the expected payload of the resulting object.</typeparam>
    /// <returns>
    /// A <see cref="ICommandMessage{TPayload}"/> containing given <paramref name="command"/> as payload, a
    /// <paramref name="command"/> if it already implements<see cref="ICommandMessage{TPayload}"/>, or a
    /// <see cref="ICommandMessage{TPayload}"/> based on the result of <seealso cref="IMessage{TPayload}.Payload"/> and
    /// <see cref="IMessage{TPayload}.MetaData"/> for other <see cref="IMessage{TPayload}"/> implementations.
    /// </returns>
    public static ICommandMessage<TMessage> AsCommandMessage<TMessage>(object command)
        where TMessage : class
    {
        if (command is ICommandMessage<TMessage> commandMessage)
        {
            return commandMessage;
        }

        if (command is IMessage<TMessage> message)
        {
            return new GenericCommandMessage<TMessage>(message.Payload, message.MetaData);
        }

        return new GenericCommandMessage<TMessage>((TMessage)command, MetaData.EmptyInstance);
    }
}
