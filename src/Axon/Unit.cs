namespace Axon;

/// <summary>
/// Represents a void type, since <see cref="System.Void"/> is not a valid return type nor reference type in C#.
/// </summary>
internal record Unit
{
    private static readonly Unit Instance = new();

    /// <summary>
    /// Gets the default value of the <see cref="Unit"/> type.
    /// </summary>
    public static ref readonly Unit Value => ref Instance;

    /// <inheritdoc />
    public virtual bool Equals(Unit? other) => true;

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode() => 0;

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString() => "()";
}
