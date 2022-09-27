namespace Axon.Messaging;

public class GenericCommandMessageTests
{
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
}
