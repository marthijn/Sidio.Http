using System.Collections;
using System.Collections.ObjectModel;
using System.Text;

namespace Sidio.Http;

public sealed class QueryParameterCollection
{
    private readonly Dictionary<EncodedQueryParameterKey, List<string>> _parameters = new(new QueryParameterKeyComparer());
    private readonly Dictionary<EncodedQueryParameterKey, ReadOnlyCollection<string>> _readOnlyValueViews =
        new(new QueryParameterKeyComparer());

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameterCollection"/> class.
    /// </summary>
    /// <param name="queryString">The query string to initialize the collection with.</param>
    public QueryParameterCollection(string? queryString = null)
    {
        Parameters = new ParametersReadOnlyView(_parameters, _readOnlyValueViews);
        InitializeFromQueryString(queryString);
    }

    /// <summary>
    /// Adds a query parameter to the collection.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="encode">Indicates whether the key and value should be URL-encoded.</param>
    /// <param name="nullValueHandling">Indicates how null values should be handled.</param>
    /// <exception cref="ArgumentException">Thrown if a parameter with the same key already exists in the collection.</exception>
    public void Add(string key, object? value, bool encode = true, NullValueHandling nullValueHandling = NullValueHandling.RemoveWhenNull)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        AddInternal(EncodedQueryParameterKey.FromString(key, encode), value, encode, nullValueHandling);
    }

    /// <summary>
    /// Tries to add a query parameter to the collection. Returns false if the key already exists.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="encode">Indicates whether the key and value should be URL-encoded.</param>
    /// <param name="nullValueHandling">Indicates how null values should be handled.</param>
    /// <returns></returns>
    public bool TryAdd(
        string key,
        object? value,
        bool encode = true,
        NullValueHandling nullValueHandling = NullValueHandling.RemoveWhenNull)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var encodedKey = EncodedQueryParameterKey.FromString(key, encode);
        if (value == null)
        {
            return nullValueHandling switch
            {
                NullValueHandling.Ignore => !_parameters.ContainsKey(encodedKey),
                NullValueHandling.Include => AddValueList(encodedKey, [string.Empty], false),
                NullValueHandling.RemoveWhenNull => !_parameters.ContainsKey(encodedKey),
                _ => throw new ArgumentOutOfRangeException(nameof(nullValueHandling), nullValueHandling, null)
            };
        }

        return AddValueList(encodedKey, ToStringList(value, encode), false);
    }

    /// <summary>
    /// Adds or replaces a query parameter in the collection.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="encode">Indicates whether the key and value should be URL-encoded.</param>
    /// <param name="nullValueHandling">Indicates how null values should be handled.</param>
    public void AddOrReplace(
        string key,
        object? value,
        bool encode = true,
        NullValueHandling nullValueHandling = NullValueHandling.RemoveWhenNull)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        AddOrReplaceInternal(EncodedQueryParameterKey.FromString(key, encode), value, encode, nullValueHandling);
    }

    /// <summary>
    /// Removes a query parameter from the collection.
    /// </summary>
    /// <param name="key">The key of the query parameter to remove.</param>
    /// <param name="encode">Indicates whether the key and value should be URL-encoded.</param>\
    public bool Remove(string key, bool encode = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return RemoveInternal(EncodedQueryParameterKey.FromString(key, encode));
    }

    /// <summary>
    /// Gets the query parameters as a read-only live view.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Parameters { get; }

    /// <summary>
    /// Gets the number of query parameters in the collection.
    /// </summary>
    public int Count => _parameters.Count;

    /// <summary>
    /// Determines whether the collection contains a parameter with the specified key.
    /// </summary>
    public bool ContainsKey(string key, bool encode = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _parameters.ContainsKey(EncodedQueryParameterKey.FromString(key, encode));
    }

    /// <summary>
    /// Tries to get the values for a query parameter key.
    /// </summary>
    public bool TryGetValues(string key, out IReadOnlyList<string> values, bool encode = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var encodedKey = EncodedQueryParameterKey.FromString(key, encode);
        if (_readOnlyValueViews.TryGetValue(encodedKey, out var readOnlyValues))
        {
            values = readOnlyValues;
            return true;
        }

        values = [];
        return false;
    }

    /// <summary>
    /// Converts the collection into a query string.
    /// </summary>
    public string ToQueryString(bool includeQuestionMark = true)
    {
        if (_parameters.Count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        if (includeQuestionMark)
        {
            builder.Append('?');
        }

        var first = true;
        foreach (var (encodedKey, values) in _parameters)
        {
            if (values.Count == 0)
            {
                if (!first)
                {
                    builder.Append('&');
                }

                builder.Append(encodedKey.Value);
                first = false;
                continue;
            }

            foreach (var value in values)
            {
                if (!first)
                {
                    builder.Append('&');
                }

                builder.Append(encodedKey.Value);
                builder.Append('=');
                builder.Append(value);
                first = false;
            }
        }

        return builder.ToString();
    }

    private static List<string> ToStringList(object? value, bool encode)
    {
        if (value == null)
        {
            return [];
        }

        if (value is string str)
        {
            return [ConvertToString(str, encode)];
        }

        if (value is IEnumerable enumerable)
        {
            var values = new List<string>();
            foreach (var item in enumerable)
            {
                values.Add(ConvertToString(item, encode));
            }

            return values;
        }

        return [ConvertToString(value, encode)];
    }

    private static string ConvertToString(object? value, bool encode)
    {
        if (value == null)
        {
            return string.Empty;
        }

        if (value is string str)
        {
            return encode ? Uri.EscapeDataString(str) : str;
        }

        var v = value.ToString() ?? string.Empty;
        return encode ? Uri.EscapeDataString(v) : v;
    }

    private void AddInternal(EncodedQueryParameterKey key, object? value, bool encodeValue = true, NullValueHandling nullValueHandling = NullValueHandling.RemoveWhenNull)
    {
        if (value == null)
        {
            switch (nullValueHandling)
            {
                case NullValueHandling.Ignore:
                    return; // Do not add the parameter
                case NullValueHandling.Include:
                    AddValueList(key, [string.Empty], true); // Add the parameter with an empty value
                    return;
                case NullValueHandling.RemoveWhenNull:
                    RemoveInternal(key);
                    return; // Remove the parameter
                default:
                    throw new ArgumentOutOfRangeException(nameof(nullValueHandling), nullValueHandling, null);
            }
        }

        AddValueList(key, ToStringList(value, encodeValue), true);
    }

    private void AddOrReplaceInternal(
        EncodedQueryParameterKey key,
        object? value,
        bool encode = true,
        NullValueHandling nullValueHandling = NullValueHandling.RemoveWhenNull)
    {
        if (_parameters.TryGetValue(key, out _))
        {
            if (value == null)
            {
                switch (nullValueHandling)
                {
                    case NullValueHandling.Ignore:
                        return; // Do not replace the parameter
                    case NullValueHandling.Include:
                        SetValueList(key, [string.Empty]); // Replace the parameter with an empty value
                        return;
                    case NullValueHandling.RemoveWhenNull:
                        RemoveInternal(key);
                        return; // Remove the parameter
                    default:
                        throw new ArgumentOutOfRangeException(nameof(nullValueHandling), nullValueHandling, null);
                }
            }

            SetValueList(key, ToStringList(value, encode));
        }
        else
        {
            AddInternal(key, value, encode, nullValueHandling);
        }
    }

    private bool RemoveInternal(EncodedQueryParameterKey key)
    {
        var removed = _parameters.Remove(key);
        if (removed)
        {
            _readOnlyValueViews.Remove(key);
        }

        return removed;
    }

    private bool AddValueList(EncodedQueryParameterKey key, List<string> values, bool throwOnDuplicate)
    {
        if (!_parameters.TryAdd(key, values))
        {
            if (throwOnDuplicate)
            {
                throw new ArgumentException($"A parameter with the key '{key}' already exists in the collection.");
            }

            return false;
        }

        _readOnlyValueViews[key] = values.AsReadOnly();
        return true;
    }

    private void SetValueList(EncodedQueryParameterKey key, List<string> values)
    {
        _parameters[key] = values;
        _readOnlyValueViews[key] = values.AsReadOnly();
    }

    private void InitializeFromQueryString(string? queryString)
    {
        if (string.IsNullOrWhiteSpace(queryString))
        {
            return;
        }

        var remaining = queryString.AsSpan();
        if (!remaining.IsEmpty && remaining[0] == '?')
        {
            remaining = remaining[1..];
        }

        while (!remaining.IsEmpty)
        {
            var separatorIndex = remaining.IndexOf('&');
            var parameter = separatorIndex >= 0 ? remaining[..separatorIndex] : remaining;
            remaining = separatorIndex >= 0 ? remaining[(separatorIndex + 1)..] : [];

            if (parameter.IsEmpty)
            {
                continue;
            }

            var keyValueSeparatorIndex = parameter.IndexOf('=');
            var encodedKeyPart = keyValueSeparatorIndex >= 0 ? parameter[..keyValueSeparatorIndex] : parameter;
            var encodedValuePart = keyValueSeparatorIndex >= 0 ? parameter[(keyValueSeparatorIndex + 1)..] : [];

            var key = Uri.UnescapeDataString(encodedKeyPart.ToString());
            var value = Uri.UnescapeDataString(encodedValuePart.ToString());
            var encodedKey = EncodedQueryParameterKey.FromString(key, true);
            var encodedValue = ConvertToString(value, true);

            if (_parameters.TryGetValue(encodedKey, out var existingValues))
            {
                existingValues.Add(encodedValue);
                continue;
            }

            AddValueList(encodedKey, [encodedValue], false);
        }
    }

    /// <summary>
    /// A read-only view of the query parameters in the collection.
    /// Using a read only dictionary property is expensive in hot paths and creates GC pressure.
    /// A read-only live view reuses the same underlying storage, so reads are cheaper and always reflect current state.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="readOnlyValueViews"></param>
    private sealed class ParametersReadOnlyView(
        Dictionary<EncodedQueryParameterKey, List<string>> parameters,
        Dictionary<EncodedQueryParameterKey, ReadOnlyCollection<string>> readOnlyValueViews)
        : IReadOnlyDictionary<string, IReadOnlyList<string>>
    {
        public IEnumerator<KeyValuePair<string, IReadOnlyList<string>>> GetEnumerator()
        {
            foreach (var entry in parameters)
            {
                yield return new KeyValuePair<string, IReadOnlyList<string>>(entry.Key.Value, readOnlyValueViews[entry.Key]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => parameters.Count;

        public IEnumerable<string> Keys
        {
            get
            {
                foreach (var key in parameters.Keys)
                {
                    yield return key.Value;
                }
            }
        }

        public IEnumerable<IReadOnlyList<string>> Values
        {
            get
            {
                foreach (var key in parameters.Keys)
                {
                    yield return readOnlyValueViews[key];
                }
            }
        }

        public bool ContainsKey(string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            return parameters.ContainsKey(EncodedQueryParameterKey.FromString(key, false));
        }

        public bool TryGetValue(string key, out IReadOnlyList<string> value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            var encodedKey = EncodedQueryParameterKey.FromString(key, false);
            if (readOnlyValueViews.TryGetValue(encodedKey, out var existing))
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
                var encodedKey = EncodedQueryParameterKey.FromString(key, false);
                if (readOnlyValueViews.TryGetValue(encodedKey, out var values))
                {
                    return values;
                }

                throw new KeyNotFoundException($"The given key '{key}' was not present in the collection.");
            }
        }
    }
}