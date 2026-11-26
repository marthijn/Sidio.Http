namespace Sidio.Http;

/// <summary>
/// Defines the contract for an HTTP request, including properties for the HTTP method, URL, headers, and content.
/// </summary>
public interface IHttpRequest
{
    /// <summary>
    /// Gets the HTTP method of the request (e.g., GET, POST, PUT, DELETE). This property can be null if the method is not specified.
    /// </summary>
    HttpMethod? Method { get; }

    /// <summary>
    /// Gets the URL of the request. This property can be null if the URL is not specified.
    /// </summary>
    IUrl? Url { get; }

    /// <summary>
    /// Gets the content of the request. This property can be null if the request does not have a body (e.g., for GET requests).
    /// </summary>
    HttpContent? Content { get; }

    /// <summary>
    /// Gets the headers for the request.
    /// </summary>
    RequestHeaderCollection Headers { get; }

    /// <summary>
    /// Gets a value indicating whether the request is valid. A request is considered valid if it has a non-null HTTP method and a non-null URL.
    /// </summary>
    /// <returns><c>true</c> if the request is valid; otherwise, <c>false</c>.</returns>
    /// <seealso cref="Validate"/>
    internal bool IsValid();

    /// <summary>
    /// Validates the request and throws an exception if it is not valid. A request is considered valid if it has a non-null HTTP method and a non-null URL.
    /// </summary>
    /// <exception cref="InvalidHttpRequestException">Thrown if the request is not valid.</exception>
    /// <seealso cref="IsValid"/>
    internal void Validate();
}