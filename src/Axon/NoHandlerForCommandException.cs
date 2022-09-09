namespace Axon;

/// <summary>
/// Exception indicating that no suitable handler could be found for the given command.
/// </summary>
public sealed class NoHandlerForCommandException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoHandlerForCommandException"/> class.
    /// </summary>
    /// <param name="message">The message describing the cause of the exception.</param>
    public NoHandlerForCommandException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoHandlerForCommandException"/> class with a message
    /// describing the given command's name.
    /// </summary>
    /// <param name="commandMessage">The command for which no handler was found.</param>
    public NoHandlerForCommandException(object commandMessage)
        : base($"No handler was subscribed to command {commandMessage.GetType().Name}")
    {
    }
}
