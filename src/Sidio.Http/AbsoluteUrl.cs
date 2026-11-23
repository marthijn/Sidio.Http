using System.Diagnostics;

namespace Sidio.Http;

/// <summary>
/// Represents an absolute URL. This record struct is used to ensure that a URL is absolute and not relative.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct AbsoluteUrl
{
    internal AbsoluteUrl(Url url)
    {
        if (!url.IsAbsolute)
        {
            throw new ArgumentException("The provided URL must be absolute.", nameof(url));
        }

        Value = url.ToString();
    }

    /// <summary>
    /// Gets the string representation of the absolute URL.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>
    /// Converts a <see cref="AbsoluteUrl"/> to a <see cref="Uri"/>.
    /// </summary>
    /// <param name="url">The URL to convert.</param>
    /// <returns>An absolute or relative URI representing the URL.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the URL cannot be represented as a URI.</exception>
    public static implicit operator Uri(AbsoluteUrl url)
    {
        return Uri.TryCreate(url.Value, UriKind.Absolute, out var absoluteUri)
            ? absoluteUri
            : throw new InvalidOperationException($"Cannot convert '{url}' to a URI.");
    }
}