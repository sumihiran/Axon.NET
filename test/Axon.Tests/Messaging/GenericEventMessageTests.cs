namespace Axon.Messaging;

public class GenericEventMessageTests
{
    [Fact]
    public void Constructors()
    {
        var payload = new object();
        GenericEventMessage<object> message1 = new(payload);
        IDictionary<string, object> metaDataMap = new Dictionary<string, object> { { "key", "value" } };
        var metaData = MetaData.From(metaDataMap);

        var message2 = new GenericEventMessage<object>(payload, metaData);
        var message3 = new GenericEventMessage<object>(payload, metaDataMap);

        Assert.Equivalent(MetaData.EmptyInstance, message1.MetaData);
        Assert.Equal(typeof(object), message1.Payload?.GetType());
        Assert.Equal(typeof(object), message1.PayloadType);

        Assert.Equivalent(metaData, message2.MetaData);
        Assert.Equal(typeof(object), message2.Payload?.GetType());
        Assert.Equal(typeof(object), message2.PayloadType);

        Assert.Equal(metaData, message3.MetaData);
        Assert.Equal(typeof(object), message3.Payload?.GetType());
        Assert.Equal(typeof(object), message3.PayloadType);

        Assert.False(message1.Identifier.Equals(message2.Identifier, StringComparison.Ordinal));
        Assert.False(message1.Identifier.Equals(message3.Identifier, StringComparison.Ordinal));
        Assert.False(message2.Identifier.Equals(message3.Identifier, StringComparison.Ordinal));
    }

    [Fact]
    public void AndMetadata()
    {
        var payload = new object();
        IDictionary<string, object> metaDataMap = new Dictionary<string, object> { { "key", "value" } };
        var metaData = MetaData.From(metaDataMap);

        var message = new GenericEventMessage<object>(payload, metaData);
        var message1 = message.AndMetaData(MetaData.EmptyInstance);
        var message2 = message.AndMetaData(
            MetaData.From(new Dictionary<string, object> { { "key", "otherValue" } }));

        Assert.Single(message1.MetaData);
        Assert.True(message1.MetaData.TryGetValue("key", out var keyValue));
        Assert.Equal("value", keyValue);

        Assert.Single(message2.MetaData);
        Assert.True(message2.MetaData.TryGetValue("key", out var keyOtherValue));
        Assert.Equal("otherValue", keyOtherValue);
    }

    [Fact]
    public void WithMetaData()
    {
        var payload = new object();
        IDictionary<string, object> metaDataMap = new Dictionary<string, object> { { "key", "value" } };
        var metaData = MetaData.From(metaDataMap);

        var message = new GenericEventMessage<object>(payload, metaData);
        var message1 = message.WithMetaData(MetaData.EmptyInstance);
        var message2 = message.WithMetaData(
            MetaData.From(new Dictionary<string, object> { { "key", "otherValue" } }));

        Assert.Empty(message1.MetaData);
        Assert.Single(message2.MetaData);
        Assert.True(message2.MetaData.TryGetValue("key", out var value));
        Assert.Equal("otherValue", value);
    }
}
