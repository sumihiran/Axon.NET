namespace Axon.Messaging;

public class GenericMessageTests
{
    [Fact]
    public void Should_ReturnValidPayloadType_WhenPayloadTypeIsNotGiven()
    {
        var message = new GenericMessage<Message>("id", new Message(), MetaData.EmptyInstance);
        Assert.Equal(typeof(Message), message.PayloadType);
    }

    [Fact]
    public void Should_ReturnsProvidedMessageAsIs_When_MessageIsProvided()
    {
        GenericMessage<string> originalMessage = new("payload");

        var result = GenericMessage.AsMessage(originalMessage);

        Assert.Equivalent(originalMessage, result);
    }

    [Fact]
    public void Should_ReturnWrappedMessage_When_ObjectIsProvided()
    {
        var payload = "payload";
        var message = GenericMessage.AsMessage(payload);

        Assert.Equal(payload, message.Payload);
    }

    [Fact]
    public void Should_Throw_ArgumentNullException_When_CommandPayloadIsNull()
    {
        var message = GenericMessage.AsMessage(null);
        var expectedException = Assert.Throws<ArgumentNullException>(() =>
        {
            _ = GenericCommandMessage.AsCommandMessage<object>(message);
        });

        Assert.Contains("Payload", expectedException.Message);
    }

    [Fact]
    public void Should_Throw_ArgumentNullException_When_EventPayloadIsNull()
    {
        var message = GenericMessage.AsMessage(null);
        var expectedException = Assert.Throws<ArgumentNullException>(() =>
        {
            _ = GenericEventMessage.AsEventMessage<object>(message);
        });

        Assert.Contains("Payload", expectedException.Message);
    }

    [Fact]
    public void Should_Throw_ArgumentNullException_When_QueryPayloadIsNull()
    {
        var message = GenericMessage.AsMessage(null);
        var expectedException = Assert.Throws<ArgumentNullException>(() =>
        {
            _ = new GenericQueryMessage<object, object>(message, nameof(message), ResponseTypes.ResponseTypes.InstanceOf<object>());
        });

        Assert.Contains("Payload", expectedException.Message);
    }

    public record Message;
}
