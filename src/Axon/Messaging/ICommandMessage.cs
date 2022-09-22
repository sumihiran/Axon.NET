namespace Axon.Messaging;

/// <summary>
/// Represents a Message carrying a command as its payload. These messages carry an intention to change application
/// state.
/// </summary>
/// <typeparam name="TPayload">The type of payload contained in the message.</typeparam>
public interface ICommandMessage<out TPayload> : IMessage<TPayload>
    where TPayload : class
{
    /// <summary>
    /// Gets the name of the command to execute. This is an indication of what should be done, using the payload as
    /// parameter.
    /// </summary>
    string CommandName { get; }

    /// <inheritdoc />
    IMessage<TPayload> IMessage<TPayload>.WithMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        this.WithMetaData(metaData);

    /// <inheritdoc />
    IMessage<TPayload> IMessage<TPayload>.AndMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        this.AndMetaData(metaData);

    /// <summary>
    /// Returns a copy of this CommandMessage with the given <paramref name="metaData"/>. The payload remains unchanged.
    /// <para/>
    /// While the implementation returned may be different than the implementation of
    /// <see cref="ICommandMessage{TPayload}"/>, implementations must take special care in returning the same type of
    /// Message to prevent errors further downstream.
    /// </summary>
    /// <param name="metaData">The new MetaData for the Message.</param>
    /// <returns>A copy of this message with the given MetaData.</returns>
    new ICommandMessage<TPayload> WithMetaData(ICollection<KeyValuePair<string, object>> metaData);

    /// <summary>
    /// Returns a copy of this CommandMessage with it MetaData merged with the given. The payload remain unchanged.
    /// </summary>
    /// <param name="metaData">The MetaData to merge with.</param>
    /// <returns>A copy of this message with the given MetaData.</returns>
    new ICommandMessage<TPayload> AndMetaData(ICollection<KeyValuePair<string, object>> metaData);
}
