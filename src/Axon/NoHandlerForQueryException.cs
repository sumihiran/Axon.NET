namespace Axon;

/// <summary>
/// Exception indicating a query for a single result was executed, but no handlers were found that could provide an
/// answer.
/// </summary>
public class NoHandlerForQueryException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoHandlerForQueryException"/> class with given
    /// <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The message explaining the context of the exception.</param>
    public NoHandlerForQueryException(string message)
        : base(message)
    {
    }
}
