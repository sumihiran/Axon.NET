namespace Axon;

/// <summary>
/// Set of extensions for Collections.
/// </summary>
internal static class CollectionExtensions
{
    /// <summary>
    /// Returns true if this collection contains no items.
    /// </summary>
    /// <param name="items">This collection.</param>
    /// <typeparam name="T">The type of the item in the collection.</typeparam>
    /// <returns>true if this collection contains no items.</returns>
    public static bool IsEmpty<T>(this ICollection<T> items) => items.Count == 0;
}
