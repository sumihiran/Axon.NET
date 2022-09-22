namespace Axon.Messaging;

/// <summary>
/// Message that represents a result from handling a <see cref="ICommandMessage{TPayload}"/>.
/// </summary>
/// <typeparam name="TResult">The type of payload contained in this Message.</typeparam>
public interface ICommandResultMessage<out TResult> : IResultMessage<TResult>
    where TResult : class
{
    /// <inheritdoc />
    IResultMessage<TResult> IResultMessage<TResult>.WithMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        this.WithMetaData(metaData);

    /// <inheritdoc />
    IResultMessage<TResult> IResultMessage<TResult>.AndMetaData(ICollection<KeyValuePair<string, object>> metaData)
        => this.AndMetaData(metaData);

    /// <summary>
    /// Returns a copy of this Message with the given <paramref name="metaData"/>. The payload remains unchanged.
    /// </summary>
    /// <param name="metaData">The new MetaData for the Message.</param>
    /// <returns>A copy of this message with the given MetaData.</returns>
    new ICommandResultMessage<TResult> WithMetaData(ICollection<KeyValuePair<string, object>> metaData);

    /// <summary>
    /// Returns a copy of this Message with it MetaData merged with the given. The payload remain unchanged.
    /// </summary>
    /// <param name="metaData">The MetaData to merge with.</param>
    /// <returns>A copy of this message with the given MetaData.</returns>
    new ICommandResultMessage<TResult> AndMetaData(ICollection<KeyValuePair<string, object>> metaData);
}
