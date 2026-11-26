using System.Collections;
using System.Collections.ObjectModel;

namespace Sidio.Http;

/// <summary>
/// Represents a collection of HTTP request headers with case-insensitive keys and support for multiple values per key.
/// </summary>
public sealed class RequestHeaderCollection
{
    private readonly Dictionary<string, List<string>> _headers = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, ReadOnlyCollection<string>> _readOnlyValueViews = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestHeaderCollection"/> class.
    /// </summary>
    public RequestHeaderCollection()
    {
        Headers = new HeadersReadOnlyView(_headers, _readOnlyValueViews);
    }

    /// <summary>
    /// Gets the headers as a read-only live view.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers { get; }

    /// <summary>
    /// Gets the number of header keys in the collection.
    /// </summary>
    public int Count => _headers.Count;

    /// <summary>
    /// Adds a header to the collection.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if the key is invalid or already exists.</exception>
    public void Add(string key, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        AddValuesInternal(key, ToStringList(value), true);
    }

    /// <summary>
    /// Tries to add a header to the collection. Returns false if the key already exists.
    /// </summary>
    public bool TryAdd(string key, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return AddValuesInternal(key, ToStringList(value), false);
    }

    /// <summary>
    /// Adds or replaces a header in the collection.
    /// </summary>
    public void AddOrReplace(string key, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        SetValuesInternal(key, ToStringList(value));
    }

    /// <summary>
    /// Removes a header from the collection.
    /// </summary>
    public bool Remove(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var removed = _headers.Remove(key);
        if (removed)
        {
            _readOnlyValueViews.Remove(key);
        }

        return removed;
    }

    /// <summary>
    /// Clears all headers from the collection.
    /// </summary>
    public void Clear()
    {
        _headers.Clear();
        _readOnlyValueViews.Clear();
    }

    /// <summary>
    /// Determines whether the collection contains a header with the specified key.
    /// </summary>
    public bool ContainsKey(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _headers.ContainsKey(key);
    }

    /// <summary>
    /// Tries to get the values for a header key.
    /// </summary>
    public bool TryGetValues(string key, out IReadOnlyList<string> values)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        if (_readOnlyValueViews.TryGetValue(key, out var readOnlyValues))
        {
            values = readOnlyValues;
            return true;
        }

        values = [];
        return false;
    }

    private bool AddValuesInternal(string key, List<string> values, bool throwOnDuplicate)
    {
        if (!_headers.TryAdd(key, values))
        {
            if (throwOnDuplicate)
            {
                throw new ArgumentException($"A header with the key '{key}' already exists in the collection.");
            }

            return false;
        }

        _readOnlyValueViews[key] = values.AsReadOnly();
        return true;
    }

    private void SetValuesInternal(string key, List<string> values)
    {
        _headers[key] = values;
        _readOnlyValueViews[key] = values.AsReadOnly();
    }

    private static List<string> ToStringList(object? value)
    {
        if (value is null)
        {
            return [string.Empty];
        }

        if (value is string str)
        {
            return [str];
        }

        if (value is IEnumerable enumerable)
        {
            var values = new List<string>();
            foreach (var item in enumerable)
            {
                values.Add(item?.ToString() ?? string.Empty);
            }

            return values;
        }

        return [value.ToString() ?? string.Empty];
    }

    private sealed class HeadersReadOnlyView(
        Dictionary<string, List<string>> headers,
        Dictionary<string, ReadOnlyCollection<string>> readOnlyValueViews)
        : IReadOnlyDictionary<string, IReadOnlyList<string>>
    {
        public IEnumerator<KeyValuePair<string, IReadOnlyList<string>>> GetEnumerator()
        {
            foreach (var entry in headers)
            {
                yield return new KeyValuePair<string, IReadOnlyList<string>>(entry.Key, readOnlyValueViews[entry.Key]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => headers.Count;

        public IEnumerable<string> Keys => headers.Keys;

        public IEnumerable<IReadOnlyList<string>> Values => readOnlyValueViews.Values;

        public bool ContainsKey(string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            return headers.ContainsKey(key);
        }

        public bool TryGetValue(string key, out IReadOnlyList<string> value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            if (readOnlyValueViews.TryGetValue(key, out var existing))
            {
                value = existing;
                return true;
            }

            value = [];
            return false;
        }

        public IReadOnlyList<string> this[string key]
        {
            get
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(key);
                if (readOnlyValueViews.TryGetValue(key, out var values))
                {
                    return values;
                }

                throw new KeyNotFoundException($"The given key '{key}' was not present in the collection.");
            }
        }
    }
}
