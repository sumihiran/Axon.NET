namespace Axon;

/// <summary>
/// A component that processes Messages.
/// </summary>
/// <typeparam name="T">The type of the Message to be handled.</typeparam>
public class MessageHandler<T>
    where T : class
{
    /// <summary>
    /// Handle the given message.
    /// </summary>
    /// <param name="message">The message to be handled.</param>
    /// <returns>The result of the message handling.</returns>
    public virtual Task<object> HandleAsync(T message) => Task.FromResult((object)message);
}
