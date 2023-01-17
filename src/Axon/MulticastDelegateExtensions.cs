namespace Axon;

/// <summary>
/// MulticastDelegate extensions.
/// </summary>
public static class MulticastDelegateExtensions
{
    /// <summary>
    /// Returns a hash for a <see cref="MulticastDelegate"/>.
    /// <para/>
    /// The method iterates through each delegate in the invocation list using a foreach loop, and XORs the hash code of
    /// the method and the target of each delegate with the running total hash code. This ensures that any two distinct
    /// action delegates will have different hash codes, even if they have the same method and target.
    /// <para />
    /// IMPORTANT: As with any hash function, hash codes are not guaranteed to be unique and a collision can occur.
    /// However, this method should provide a good starting point for generating unique hash codes for action delegates.
    /// </summary>
    /// <param name="delegate">The <see cref="MulticastDelegate"/>.</param>
    /// <returns>The hash value.</returns>
    public static int GetMulticastDelegateHashCode(this MulticastDelegate @delegate)
    {
        var hash = @delegate.GetType().GetHashCode();
        foreach (var d in @delegate.GetInvocationList())
        {
            hash ^= d.Method.GetHashCode();
            if (d.Target is not null)
            {
                hash ^= d.Target.GetHashCode();
            }
        }

        return hash;
    }
}
