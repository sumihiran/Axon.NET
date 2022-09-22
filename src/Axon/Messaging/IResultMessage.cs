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

    /// <inheritdoc />
    IMessage<TResult> IMessage<TResult>.WithMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        this.WithMetaData(metaData);

    /// <inheritdoc />
    IMessage<TResult> IMessage<TResult>.AndMetaData(ICollection<KeyValuePair<string, object>> metaData) =>
        this.AndMetaData(metaData);

    /// <summary>
    /// Returns a copy of this Message with the given <paramref name="metaData"/>. The payload remains unchanged.
    /// </summary>
    /// <param name="metaData">The new MetaData for the Message.</param>
    /// <returns>A copy of this message with the given MetaData.</returns>
    new IResultMessage<TResult> WithMetaData(ICollection<KeyValuePair<string, object>> metaData);

    /// <summary>
    /// Returns a copy of this Message with it MetaData merged with the given. The payload remain unchanged.
    /// </summary>
    /// <param name="metaData">The MetaData to merge with.</param>
    /// <returns>A copy of this message with the given MetaData.</returns>
    new IResultMessage<TResult> AndMetaData(ICollection<KeyValuePair<string, object>> metaData);
}
