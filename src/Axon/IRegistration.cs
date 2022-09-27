namespace Axon;

/// <summary>
/// Interface that provides a mechanism to cancel a registration.
/// </summary>
public interface IRegistration : IAsyncDisposable
{
    /// <summary>
    /// Cancels this Registration. If the Registration was already cancelled, no action is taken.
    /// </summary>
    /// <returns>
    /// <c>true</c> if this handler is successfully unregistered, <c>false</c> if this handler was not currently
    /// registered.
    /// </returns>
    Task<bool> CancelAsync();
}
