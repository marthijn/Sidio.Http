using System.Diagnostics;

namespace Sidio.Http;

[DebuggerDisplay("{Method} {Url}")]
public sealed partial class HttpRequest : IHttpRequest
{
    public HttpRequest(HttpMethod? method = null, IUrl? url = null, HttpContent? content = null)
    {
        Url = url;
        Method = method;
        Content = content;
    }

    /// <inheritdoc />
    public HttpMethod? Method { get; private set;  }

    /// <inheritdoc />
    public IUrl? Url { get; private set; }

    /// <inheritdoc />
    public HttpContent? Content { get; private set; }

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