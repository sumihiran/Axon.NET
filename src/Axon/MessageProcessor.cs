namespace Axon;

/// <summary>
/// The The message processor delegate.
/// </summary>
/// <param name="messages">The collection of message to process.</param>
/// <typeparam name="TMessage">The type of the message.</typeparam>
/// <returns>A <see cref="Task"/> that represents the asynchronous processing.</returns>
public delegate Task MessageProcessor<TMessage>(List<TMessage> messages);
