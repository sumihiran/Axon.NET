namespace Axon;

using Axon.Messaging;

/// <summary>
/// Callback that does absolutely nothing when invoked.
/// </summary>
public static class NoOpCallback
{
    /// <summary>
    /// Gets a statically available delegate of the NoOpCallback. Provided for convenience.
    /// </summary>
    public static CommandCallback<object, object> Instance => OnResultAsync;

    /// <summary>
    /// Invoked when command handling execution is completed. This implementation cause the task to complete regardless.
    /// </summary>
    /// <param name="commandMessage">The <see cref="ICommandMessage{TPayload}"/> that was dispatched.</param>
    /// <param name="commandResultMessage">The CommandResultMessage of the command handling execution.</param>
    /// <returns>A completed <see cref="Task"/>.</returns>
    public static Task OnResultAsync(
        ICommandMessage<object> commandMessage, ICommandResultMessage<object> commandResultMessage) =>
        Task.CompletedTask;
}
