using System.Diagnostics;

namespace Sidio.Http;

/// <summary>
/// Represents an HTTP request with method, URL, content, and headers.
/// </summary>
[DebuggerDisplay("{Method} {Url}")]
public sealed partial class HttpRequest : IHttpRequest
{
    private RequestHeaderCollection? _headers;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequest"/> class with the specified HTTP method, URL, content, and headers.
    /// </summary>
    /// <param name="method">The HTTP method of the request.</param>
    /// <param name="url">The URL of the request.</param>
    /// <param name="content">The content of the request.</param>
    /// <param name="headers">The headers of the request.</param>
    public HttpRequest(HttpMethod? method = null, IUrl? url = null, HttpContent? content = null, RequestHeaderCollection? headers = null)
    {
        Url = url;
        Method = method;
        Content = content;
        _headers = headers;
    }

    /// <inheritdoc />
    public HttpMethod? Method { get; private set;  }

    /// <inheritdoc />
    public IUrl? Url { get; private set; }

    /// <inheritdoc />
    public HttpContent? Content { get; private set; }

    /// <inheritdoc />
    public RequestHeaderCollection Headers => _headers ??= new RequestHeaderCollection();

    /// <inheritdoc />
    public bool IsValid()
    {
        return Method is not null && Url is not null;
    }

    /// <inheritdoc />
    public void Validate()
    {
        if (Method is null)
        {
            throw new InvalidHttpRequestException("HTTP method is not specified.");
        }

        if (Url is null)
        {
            throw new InvalidHttpRequestException("URL is not specified.");
        }
    }
}