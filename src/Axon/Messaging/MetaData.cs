namespace Axon.Messaging;

using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents MetaData that is passed along with a payload in a Message.
/// </summary>
[SuppressMessage("ReSharper", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "MetaData is a dictionary.")]
public class MetaData : IReadOnlyDictionary<string, object>, ICollection<KeyValuePair<string, object>>,
    IEquatable<ICollection<KeyValuePair<string, object>>>
{
    /// <summary>
    /// Gets an empty MetaData instance.
    /// </summary>
    public static readonly MetaData EmptyInstance = new();

    private readonly IReadOnlyDictionary<string, object> values;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetaData"/> class.
    /// </summary>
    public MetaData() => this.values = new Dictionary<string, object>();

    /// <summary>
    /// Initializes a new instance of the <see cref="MetaData"/> class with given <paramref name="items"/> as content.
    /// Note that the items are copied into the MetaData. Modifications in the Map of items will not reflect is the
    /// MetaData, or vice versa. Modifications in the items themselves <em>are</em> reflected in the MetaData.
    /// </summary>
    /// <param name="items">The items to populate the MetaData with.</param>
    public MetaData(IEnumerable<KeyValuePair<string, object>> items) => this.values = items.ToImmutableDictionary();

    /// <summary>
    /// Gets the number of metadata items.
    /// </summary>
    public int Count => this.values.Count;

    /// <inheritdoc />
    public bool IsReadOnly => true;

    /// <summary>
    /// Gets a value indicating whether the MetaData is empty.
    /// </summary>
    public bool IsEmpty => this.Count == 0;

    /// <inheritdoc />
    public IEnumerable<string> Keys => this.values.Keys;

    /// <inheritdoc />
    public IEnumerable<object> Values => this.values.Values;

    /// <inheritdoc />
    public object this[string key] => this.values[key];

    /// <summary>
    /// Creates a new MetaData instance from the given <paramref name="metaDataEntries"/>.
    /// </summary>
    /// <param name="metaDataEntries">The items to populate the MetaData with.</param>
    /// <returns>A MetaData instance with the given <paramref name="metaDataEntries"/> as content.</returns>
    public static MetaData From(ICollection<KeyValuePair<string, object>> metaDataEntries)
    {
        if (metaDataEntries.IsEmpty())
        {
            return EmptyInstance;
        }

        return new MetaData(metaDataEntries);
    }

    /// <summary>
    /// Creates a MetaData instances with a single entry, with the given <paramref name="key"/>
    /// and <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key for the entry.</param>
    /// <param name="value">The value for the entry.</param>
    /// <returns>A MetaData instance with a single entry.</returns>
    public static MetaData With(string key, object value)
    {
        var metadata = new Dictionary<string, object>(1) { { key, value } };
        return new MetaData(metadata);
    }

    /// <summary>
    /// Returns a MetaData instances containing the current entries, and given <paramref name="key"/>
    /// and <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key for the entry.</param>
    /// <param name="value">The value for the entry.</param>
    /// <returns>A MetaData instance with an additional entry.</returns>
    public MetaData And(string key, object value)
    {
        var newValues = new Dictionary<string, object>(this.values) { { key, value } };
        return new MetaData(newValues);
    }

    /// <inheritdoc />
    public void Add(KeyValuePair<string, object> item) => throw new NotImplementedException();

    /// <inheritdoc />
    public void Clear() => throw new NotImplementedException();

    /// <inheritdoc />
    public bool Contains(KeyValuePair<string, object> item) => this.Values.Contains(item);

    /// <inheritdoc />
    /// TODO: Implement Metadata.CopyTo
    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        => this.values.ToArray().CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(KeyValuePair<string, object> item) => throw new NotImplementedException();

    /// <inheritdoc />
    public bool ContainsKey(string key) => this.values.ContainsKey(key);

    /// <inheritdoc />
    public bool TryGetValue(string key, out object value) => this.values.TryGetValue(key, out value!);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this.values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <inheritdoc />
    public bool Equals(ICollection<KeyValuePair<string, object>>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (this.values.Count != other.Count)
        {
            return false;
        }

        return this.values.OrderBy(item => item.Key)
            .SequenceEqual(other.OrderBy(item => item.Key));
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return this.Equals((MetaData)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode() => this.values.GetHashCode();

    /// <summary>
    /// Returns a MetaData instance containing values of current metaData, combined with the given
    /// <paramref name="additionalEntries"/>. If any entries have identical keys, the values from the
    /// <paramref name="additionalEntries"/> will take precedence.
    /// </summary>
    /// <param name="additionalEntries">The additional entries for the new MetaData.</param>
    /// <returns>
    /// A MetaData instance containing values of current metaData, combined with the given
    /// <paramref name="additionalEntries"/>.
    /// </returns>
    public MetaData MergedWith(ICollection<KeyValuePair<string, object>> additionalEntries)
    {
        if (additionalEntries.IsEmpty())
        {
            return this;
        }

        if (this.IsEmpty)
        {
            return From(additionalEntries);
        }

        var additionalEntryKeys = additionalEntries.Select(kv => kv.Key).ToImmutableHashSet();
        return new MetaData(additionalEntries.Union(this.values.Where(kv => !additionalEntryKeys.Contains(kv.Key))));
    }

    /// <summary>
    /// Compares the specified object with this entry for equality.
    /// </summary>
    /// <param name="other">MetaData to be compared for equality with this MetaData.</param>
    /// <returns>True if the specified object is equal to this dictionary entry.</returns>
    protected bool Equals(MetaData other) => this.values.Equals(other.values);
}
