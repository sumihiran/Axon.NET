namespace Axon.Messaging;

public class GenericResultMessageTest
{
    private static readonly OpsException Exception = new("ops");

    [Fact]
    public void IsSuccess_Should_ReturnFalse_WhenExceptional()
    {
        var resultMessage = GenericResultMessage.AsResultMessage(Exception);

        Assert.False(resultMessage.IsSuccess);
        Assert.Equal(Exception, resultMessage.Exception);
    }

    [Fact]
    public void PayloadType_Should_ReturnType_WhenExceptional()
    {
        var expectedType = typeof(string);
        var resultMessage = GenericResultMessage.AsErrorResultMessage<string>(Exception);

        Assert.Equal(expectedType, resultMessage.PayloadType);
    }

    private class OpsException : Exception
    {
        public OpsException(string? message)
            : base(message)
        {
        }
    }
}
