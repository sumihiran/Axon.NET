namespace Axon;

using Axon.Messaging;

public class WrappedMessageHandlerCallbackTest
{
    [Fact]
    public void GetHashCode_Should_ReturnUniqueValue()
    {
        // Arrange
        MessageHandlerCallback<IMessage<object>> handler1 = message => Task.FromResult<object?>("Hello");
        MessageHandlerCallback<IMessage<object>> handler2 = message => Task.FromResult<object?>("Hello");

        var wrappedHandler1 = new WrappedMessageHandlerCallback<IMessage<object>>(handler1);
        var wrappedHandler2 = new WrappedMessageHandlerCallback<IMessage<object>>(handler2);

        // Assert
        Assert.Equal(handler1.GetHashCode(), handler2.GetHashCode());
        Assert.NotEqual(handler1.GetMulticastDelegateHashCode(), handler2.GetMulticastDelegateHashCode());
        Assert.NotEqual(wrappedHandler1.GetHashCode(), wrappedHandler2.GetHashCode());
    }
}
