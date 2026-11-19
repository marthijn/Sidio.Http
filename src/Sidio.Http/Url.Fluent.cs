using System;
using System.Linq;

namespace Sidio.Http;

public sealed partial class Url
{
    /// <summary>
    /// Sets the host for the URL. If the host is invalid, an ArgumentException will be thrown.
    /// </summary>
    /// <remarks>Only the host part of the URL will be set. The scheme and port will remain unchanged.</remarks>
    /// <param name="host">The host to set.</param>
    /// <returns>The updated URL instance.</returns>
    public Url WithHost(string? host)
    {
        Host = host;
        return this;
    }

    /// <summary>
    /// Sets the scheme for the URL. Valid schemes are "http" and "https". If an invalid scheme is provided, an ArgumentException will be thrown.
    /// </summary>
    /// <param name="scheme">The scheme to set.</param>
    /// <returns>The updated URL instance.</returns>
    public Url WithScheme(string? scheme)
    {
        Scheme = scheme;
        return this;
    }

    /// <summary>
    /// Sets the port for the URL. If the port is the default for the scheme (80 for HTTP, 443 for HTTPS), it will be set to null.
    /// </summary>
    /// <param name="port">The port number to set.</param>
    /// <returns>The updated URL instance.</returns>
    public Url WithPort(int? port)
    {
        Port = port;
        return this;
    }

    /// <summary>
    /// Appends a path segment to the URL.
    /// </summary>
    /// <param name="pathSegments">The path segments to append.</param>
    /// <returns>The updated URL instance.</returns>
    public Url AppendPath(params string[] pathSegments)
    {
        foreach (var segment in pathSegments.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            AppendPathInternal(segment);
        }

        return this;

        void AppendPathInternal(string pathSegment)
        {
            if (string.IsNullOrWhiteSpace(pathSegment))
            {
                throw new ArgumentException("Path segment cannot be null or whitespace.", nameof(pathSegment));
            }

            var splitSegments = pathSegment.Trim(PathSeparator).Split(
                [PathSeparator],
                StringSplitOptions.RemoveEmptyEntries);
            _pathSegments.AddRange(splitSegments);
        }
    }
}