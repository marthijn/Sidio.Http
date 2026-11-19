using System;
using System.Collections.Generic;

namespace Sidio.Http;

public sealed partial class Url
{
    private const char PathSeparator = '/';

    private readonly List<string> _pathSegments = new();

    public Url()
    {
    }

    public string? Scheme
    {
        get;
        private set
        {
            var trimmedValue = value?.Trim().ToLowerInvariant();
            if (trimmedValue is not null && !trimmedValue.Equals("http", StringComparison.OrdinalIgnoreCase) && !trimmedValue.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Scheme must be either 'http' or 'https'.", nameof(value));
            }

            field = trimmedValue;
        }
    }

    /// <summary>
    /// Gets the host of the URL.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the host is invalid.</exception>
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
                if (!hostToCheck.StartsWith("//")
                    && !hostToCheck.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                    && !hostToCheck.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    // Prepend scheme for validation
                    hostToCheck = "https://" + hostToCheck;
                }

                if (!Uri.CheckHostName(hostToCheck).Equals(UriHostNameType.Unknown))
                {
                    throw new ArgumentException("Host must be a valid hostname or IP address.", nameof(value));
                }

                var uri = new Uri(hostToCheck, UriKind.Absolute);
                trimmedValue = uri.Host;
            }

            field = trimmedValue;
        }
    }

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

    public IReadOnlyList<string> PathSegments => _pathSegments.AsReadOnly();

    public static Url From(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL cannot be null or whitespace.", nameof(url));
        }

        return Create(new Uri(url, UriKind.RelativeOrAbsolute), url);
    }

    public static Url From(Uri uri)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri), "URI cannot be null.");
        }

        return Create(uri, uri.OriginalString);
    }

    private static Url Create(Uri uri, string originalString)
    {
        var url = new Url()
            .WithHost(uri.Host)
            .WithScheme(uri.Scheme)
            .WithPort(uri.Port)
            .AppendPath(uri.AbsolutePath);
        return url;
    }
}