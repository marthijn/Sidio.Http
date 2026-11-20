namespace Sidio.Http;
using System.Text;

/// <summary>
/// Represents a URL and provides methods to manipulate its components, including scheme,
/// host, port, path segments, and query parameters.
/// </summary>
public sealed partial class Url
{
    private const char PathSeparator = '/';

    private readonly List<string> _pathSegments = new();
    private QueryParameterCollection? _queryParameters;

    /// <summary>
    /// Gets the scheme of the URL. Valid values are "http" and "https". If not set, it will be null.
    /// </summary>
    public string? Scheme
    {
        get;
        private set
        {
            var trimmedValue = value?.Trim();
            if (trimmedValue is not null && !trimmedValue.Equals("http", StringComparison.OrdinalIgnoreCase) &&
                !trimmedValue.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Scheme must be either 'http' or 'https'.", nameof(value));
            }

            field = trimmedValue;
        }
    }

    /// <summary>
    /// Gets the host of the URL.
    /// </summary>
    public string? Host
    {
        get;
        private set
        {
            var trimmedValue = value?.Trim();
            if (trimmedValue is not null && trimmedValue.Contains(' '))
            {
                throw new ArgumentException("Host cannot contain spaces.", nameof(value));
            }

            if (trimmedValue is not null)
            {
                if (trimmedValue.Length == 0)
                {
                    throw new ArgumentException("Host cannot be an empty string.", nameof(value));
                }

                if (trimmedValue.Length > 255)
                {
                    throw new ArgumentException("Host cannot exceed 255 characters.", nameof(value));
                }

                var hostToCheck = trimmedValue;
                if (hostToCheck.StartsWith("//", StringComparison.Ordinal))
                {
                    hostToCheck = "https:" + hostToCheck;
                }
                else if (!hostToCheck.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                    && !hostToCheck.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    hostToCheck = "https://" + hostToCheck;
                }

                if (!Uri.TryCreate(hostToCheck, UriKind.Absolute, out var uri))
                {
                    throw new ArgumentException("Host must be a valid hostname or IP address.", nameof(value));
                }

                var hostNameToCheck = uri.Host.Trim('[', ']');
                if (Uri.CheckHostName(hostNameToCheck).Equals(UriHostNameType.Unknown))
                {
                    throw new ArgumentException("Host must be a valid hostname or IP address.", nameof(value));
                }

                trimmedValue = uri.Host;
            }

            field = trimmedValue;
        }
    }

    /// <summary>
    /// Gets the port of the URL. If the port is the default for the scheme (80 for HTTP, 443 for HTTPS), it will be null.
    /// </summary>
    public int? Port
    {
        get;
        private set
        {
            if (value is < 0 or > 65535)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Port must be between 0 and 65535.");
            }

            if (value is 80 && Scheme?.Equals("http", StringComparison.OrdinalIgnoreCase) == true)
            {
                field = null; // Default port for HTTP, so we can ignore it.
            }
            else if (value is 443 && Scheme?.Equals("https", StringComparison.OrdinalIgnoreCase) == true)
            {
                field = null; // Default port for HTTPS, so we can ignore it.
            }
            else
            {
                field = value;
            }
        }
    }

    /// <summary>
    /// Gets the path segments of the URL.
    /// </summary>
    public IReadOnlyList<string> PathSegments => _pathSegments.AsReadOnly();

    /// <summary>
    /// Gets the query parameters of the URL.
    /// </summary>
    public QueryParameterCollection QueryParameters
    {
        get
        {
            _queryParameters ??= new QueryParameterCollection();
            return _queryParameters;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the URL is absolute (has both a scheme and a host).
    /// </summary>
    public bool IsAbsolute => !string.IsNullOrEmpty(Scheme) && !string.IsNullOrEmpty(Host);

    /// <inheritdoc />
    public override string ToString()
    {
        var builder = new StringBuilder();

        if (!string.IsNullOrEmpty(Scheme))
        {
            builder.Append(Scheme);
            builder.Append("://");
        }

        if (!string.IsNullOrEmpty(Host))
        {
            builder.Append(Host);
        }

        if (Port.HasValue)
        {
            builder.Append(':');
            builder.Append(Port.Value);
        }

        foreach (var segment in _pathSegments)
        {
            builder.Append(PathSeparator);
            builder.Append(Uri.EscapeDataString(segment));
        }

        if ( _queryParameters is {Count: > 0})
        {
            var queryString = _queryParameters.ToQueryString(false);
            builder.Append('?');
            builder.Append(queryString);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Creates a new <see cref="Url"/> instance from the specified URL string.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Url From(string url)
    {
        return string.IsNullOrWhiteSpace(url)
            ? throw new ArgumentException("URL cannot be null or whitespace.", nameof(url))
            : Create(new Uri(url, UriKind.RelativeOrAbsolute));
    }

    /// <summary>
    /// Creates a new <see cref="Url"/> instance from the specified <see cref="Uri"/> object.
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Url From(Uri uri)
    {
        return uri == null ? throw new ArgumentNullException(nameof(uri), "URI cannot be null.") : Create(uri);
    }

    private static Url Create(Uri uri)
    {
        var url = new Url();

        if (uri.IsAbsoluteUri)
        {
            if (!string.IsNullOrEmpty(uri.Scheme))
            {
                url.WithScheme(uri.Scheme);
            }

            if (!string.IsNullOrEmpty(uri.Host))
            {
                url.WithHost(uri.Host);
            }

            if (uri.Port >= 0)
            {
                url.WithPort(uri.Port);
            }

            if (!string.IsNullOrEmpty(uri.AbsolutePath) && uri.AbsolutePath != "/")
            {
                var decodedPath = Uri.UnescapeDataString(uri.AbsolutePath);
                url.AppendPath(decodedPath);
            }

            if (!string.IsNullOrEmpty(uri.Query))
            {
                url._queryParameters = new QueryParameterCollection(uri.Query);
            }
        }
        else
        {
            var queryIndex = uri.OriginalString.IndexOf('?');
            var pathPart = queryIndex >= 0 ? uri.OriginalString.Substring(0, queryIndex) : uri.OriginalString;
            var queryPart = queryIndex >= 0 ? uri.OriginalString.Substring(queryIndex + 1) : null;

            if (!string.IsNullOrEmpty(pathPart))
            {
                var decodedPath = Uri.UnescapeDataString(pathPart);
                url.AppendPath(decodedPath);
            }

            if (!string.IsNullOrEmpty(queryPart))
            {
                url._queryParameters = new QueryParameterCollection(queryPart);
            }
        }

        return url;
    }
}