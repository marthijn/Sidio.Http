namespace Sidio.Http;

/// <summary>
/// Defines the contract for a URL.
/// </summary>
public interface IUrl
{
    /// <summary>
    /// Gets the string representation of the URL.
    /// </summary>
    string Value { get; }
}