namespace Axon;

/// <summary>
/// Wrapper around cancellation delegate.
/// </summary>
public class Registration : IRegistration
{
    private readonly Func<Task<bool>> cancelAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="Registration"/> class.
    /// </summary>
    /// <param name="cancelAction">The cancellation function.</param>
    public Registration(Func<bool> cancelAction)
        : this(() => Task.FromResult(cancelAction()))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Registration"/> class.
    /// </summary>
    /// <param name="cancelAction">The asynchronous cancellation function.</param>
    public Registration(Func<Task<bool>> cancelAction) => this.cancelAction = cancelAction;

    /// <inheritdoc />
    public Task<bool> CancelAsync() => this.cancelAction();

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await this.CancelAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}
