namespace Sidio.Http;

public sealed class QueryParameterCollection
{
    private readonly Dictionary<EncodedQueryParameterKey, List<string>> _parameters = new(new QueryParameterKeyComparer());

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameterCollection"/> class.
    /// </summary>
    /// <param name="queryString">The query string to initialize the collection with.</param>
    public QueryParameterCollection(string? queryString = null)
    {
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
        if (_parameters.ContainsKey(encodedKey))
        {
            return false;
        }

        AddInternal(encodedKey, value, encode, nullValueHandling);
        return true;
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
    /// Gets the query parameters as a read-only dictionary.
    /// </summary>
    public IReadOnlyDictionary<string, List<string>> Parameters => _parameters.ToDictionary(
        kvp => kvp.Key.Value,
        kvp => kvp.Value,
        StringComparer.OrdinalIgnoreCase);

    private static List<string> ToStringList(object? value, bool encode)
    {
        if (value == null)
        {
            return [];
        }

        if (value is IEnumerable<object> enumerable)
        {
            return enumerable.Select(v => ConvertToString(v, encode)).ToList();
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
        if (_parameters.ContainsKey(key))
        {
            throw new ArgumentException($"A parameter with the key '{key}' already exists in the collection.");
        }

        if (value == null)
        {
            switch (nullValueHandling)
            {
                case NullValueHandling.Ignore:
                    return; // Do not add the parameter
                case NullValueHandling.Include:
                    _parameters.Add(key, [string.Empty]); // Add the parameter with an empty value
                    return;
                case NullValueHandling.RemoveWhenNull:
                    RemoveInternal(key);
                    return; // Remove the parameter
                default:
                    throw new ArgumentOutOfRangeException(nameof(nullValueHandling), nullValueHandling, null);
            }
        }

        _parameters.Add(key, ToStringList(value, encodeValue));
    }

    private void AddOrReplaceInternal(
        EncodedQueryParameterKey key,
        object? value,
        bool encode = true,
        NullValueHandling nullValueHandling = NullValueHandling.RemoveWhenNull)
    {
        if (_parameters.ContainsKey(key))
        {
            if (value == null)
            {
                switch (nullValueHandling)
                {
                    case NullValueHandling.Ignore:
                        return; // Do not replace the parameter
                    case NullValueHandling.Include:
                        _parameters[key] = [string.Empty]; // Replace the parameter with an empty value
                        return;
                    case NullValueHandling.RemoveWhenNull:
                        RemoveInternal(key);
                        return; // Remove the parameter
                    default:
                        throw new ArgumentOutOfRangeException(nameof(nullValueHandling), nullValueHandling, null);
                }
            }

            _parameters[key] = ToStringList(value, encode);
        }
        else
        {
            AddInternal(key, value, encode, nullValueHandling);
        }
    }

    private bool RemoveInternal(EncodedQueryParameterKey key)
    {
        return _parameters.Remove(key);
    }
    private void InitializeFromQueryString(string? queryString)
    {
        if (string.IsNullOrWhiteSpace(queryString))
        {
            return;
        }

        var queryParameters = queryString.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (var parameter in queryParameters)
        {
            var keyValue = parameter.Split('=', 2);
            var key = Uri.UnescapeDataString(keyValue[0]);
            var value = keyValue.Length > 1 ? Uri.UnescapeDataString(keyValue[1]) : string.Empty;

            AddInternal(EncodedQueryParameterKey.FromString(key, true), value, true, NullValueHandling.Include);
        }
    }
}