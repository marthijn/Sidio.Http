namespace Sidio.Http;

public sealed partial class Url
{
    /// <summary>
    /// Sets the host for the URL. If the host is invalid, an ArgumentException will be thrown.
    /// </summary>
    /// <remarks>Only the host part of the URL will be set. The scheme and port will remain unchanged.</remarks>
    /// <param name="host">The host to set.</param>
    /// <returns>The updated <see cref="Url"/> instance.</returns>
    public Url WithHost(string? host)
    {
        Host = host;
        return this;
    }

    /// <summary>
    /// Sets the scheme for the URL. Valid schemes are "http" and "https". If an invalid scheme is provided, an ArgumentException will be thrown.
    /// </summary>
    /// <param name="scheme">The scheme to set.</param>
    /// <returns>The updated <see cref="Url"/> instance.</returns>
    public Url WithScheme(string? scheme)
    {
        Scheme = scheme;
        return this;
    }

    /// <summary>
    /// Sets the port for the URL. If the port is the default for the scheme (80 for HTTP, 443 for HTTPS), it will be set to null.
    /// </summary>
    /// <param name="port">The port number to set.</param>
    /// <returns>The updated <see cref="Url"/> instance.</returns>
    public Url WithPort(int? port)
    {
        Port = port;
        return this;
    }

    /// <summary>
    /// Appends a path segment to the URL.
    /// </summary>
    /// <param name="pathSegments">The path segments to append.</param>
    /// <returns>The updated <see cref="Url"/> instance.</returns>
    public Url AppendPath(params string[] pathSegments)
    {
        foreach (var segment in pathSegments)
        {
            if (string.IsNullOrWhiteSpace(segment))
            {
                continue;
            }

            AppendPathInternal(segment);
        }

        return this;

        void AppendPathInternal(string pathSegment)
        {
            var remaining = pathSegment.AsSpan();
            while (!remaining.IsEmpty)
            {
                while (!remaining.IsEmpty && remaining[0] == PathSeparator)
                {
                    remaining = remaining[1..];
                }

                if (remaining.IsEmpty)
                {
                    break;
                }

                var separatorIndex = remaining.IndexOf(PathSeparator);
                var segment = separatorIndex >= 0 ? remaining[..separatorIndex] : remaining;
                if (!segment.IsEmpty)
                {
                    _pathSegments.Add(Uri.UnescapeDataString(segment.ToString()));
                }

                remaining = separatorIndex >= 0 ? remaining[(separatorIndex + 1)..] : [];
            }
        }
    }

    /// <summary>
    /// Clears all path segments from the URL.
    /// </summary>
    /// <returns>The updated <see cref="Url"/> instance.</returns>
    public Url ClearPath()
    {
        _pathSegments.Clear();
        return this;
    }

    /// <summary>
    /// Sets the path segments for the URL, replacing any existing segments.
    /// </summary>
    /// <param name="pathSegments"></param>
    /// <returns>The updated <see cref="Url"/> instance.</returns>
    public Url SetPath(params string[] pathSegments)
    {
        _pathSegments.Clear();
        return AppendPath(pathSegments);
    }

    /// <summary>
    /// Sets the query parameters for the URL, replacing any existing parameters.
    /// </summary>
    /// <param name="queryParameters"></param>
    /// <returns>The updated <see cref="Url"/> instance.</returns>
    public Url WithQueryParameters(QueryParameterCollection queryParameters)
    {
        ArgumentNullException.ThrowIfNull(queryParameters);
        _queryParameters = queryParameters;
        return this;
    }

    /// <summary>
    /// Sets a query parameter for the URL. If the value is null, the parameter will be removed if it exists.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="encode"></param>
    /// <param name="nullValueHandling"></param>
    /// <returns>The updated <see cref="Url"/> instance.</returns>
    public Url WithQueryParameter(string key, object? value, bool encode = true, NullValueHandling nullValueHandling = NullValueHandling.RemoveWhenNull)
    {
        _queryParameters ??= new QueryParameterCollection();
        _queryParameters.AddOrReplace(key, value, encode, nullValueHandling);
        return this;
    }

    /// <summary>
    /// Adds a query parameter to the URL. If the value is null, the parameter will be removed if it exists.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="encode"></param>
    /// <param name="nullValueHandling"></param>
    /// <returns>The updated <see cref="Url"/> instance.</returns>
    public Url AddQueryParameter(string key, object? value, bool encode = true, NullValueHandling nullValueHandling = NullValueHandling.RemoveWhenNull)
    {
        _queryParameters ??= new QueryParameterCollection();
        _queryParameters.TryAdd(key, value, encode, nullValueHandling);
        return this;
    }

    /// <summary>
    /// Removes a query parameter from the URL. If the parameter does not exist, no action is taken.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="encode"></param>
    /// <returns>The updated <see cref="Url"/> instance.</returns>
    public Url RemoveQueryParameter(string key, bool encode = true)
    {
        if (_queryParameters is null)
        {
            return this;
        }

        _queryParameters.Remove(key, encode);
        return this;
    }

    /// <summary>
    /// Clears all query parameters from the URL.
    /// </summary>
    /// <returns>The updated <see cref="Url"/> instance.</returns>
    public Url ClearQueryParameters()
    {
        _queryParameters = null;
        return this;
    }
}