namespace Axon.Messaging;

/// <summary>
/// Representation of a Message, containing a Payload and MetaData. Typical examples of Messages are Commands, Events and
/// Queries.
/// </summary>
/// <typeparam name="TPayload">The type of payload contained in this Message.</typeparam>
public interface IMessage<out TPayload>
    where TPayload : class
{
    /// <summary>
    /// Gets the identifier of this message.
    /// </summary>
    string Identifier { get; }

    /// <summary>
    /// Gets the metadata for this message. This meta data is a collection of key-value pair.
    /// </summary>
    MetaData MetaData { get; }

    /// <summary>
    /// Gets the payload of this message. The payload is the application-specific information.
    /// </summary>
    TPayload? Payload { get; }

    /// <summary>
    /// Gets the type of the payload.
    /// </summary>
    Type PayloadType { get; }

    /// <summary>
    /// Returns a copy of this Message with the given <paramref name="metaData"/>. The payload remains unchanged.
    /// </summary>
    /// <param name="metaData">The new MetaData for the Message.</param>
    /// <returns>A copy of this message with the given MetaData.</returns>
    IMessage<TPayload> WithMetaData(ICollection<KeyValuePair<string, object>> metaData);

    /// <summary>
    /// Returns a copy of this Message with it MetaData merged with the given. The payload remain unchanged.
    /// </summary>
    /// <param name="metaData">The MetaData to merge with.</param>
    /// <returns>A copy of this message with the given MetaData.</returns>
    IMessage<TPayload> AndMetaData(ICollection<KeyValuePair<string, object>> metaData);
}
