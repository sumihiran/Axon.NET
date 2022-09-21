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
    public void Should_ReturnVoidTypeAsPayloadType_When_PayloadIsNullable()
    {
        string? payload = null;
        var message = GenericMessage.AsMessage(payload);

        Assert.Equal(typeof(void), message.PayloadType);
    }

    public record Message;
}
