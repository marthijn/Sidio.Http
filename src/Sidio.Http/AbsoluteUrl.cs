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
}