namespace Sidio.Http;

public sealed partial class HttpRequest
{
    /// <summary>
    /// Sets the headers of the request. Any existing headers will be replaced with the new headers.
    /// </summary>
    /// <param name="headers">The header collection.</param>
    /// <returns>The current <see cref="HttpRequest"/> instance.</returns>
    public HttpRequest WithHeaders(RequestHeaderCollection headers)
    {
        ArgumentNullException.ThrowIfNull(headers);

        foreach(var header in headers.Headers)
        {
            Headers.AddOrReplace(header.Key, header.Value);
        }

        return this;
    }

    /// <summary>
    /// Adds or replaces a header in the request. If the header already exists, its value will be
    /// replaced with the new value. If the header does not exist, it will be added to the request.
    /// </summary>
    /// <param name="key">The header key.</param>
    /// <param name="value">The header value.</param>
    /// <returns>The current <see cref="HttpRequest"/> instance.</returns>
    public HttpRequest WithHeader(string key, object? value)
    {
        Headers.AddOrReplace(key, value);
        return this;
    }

    /// <summary>
    /// Adds a header to the request if it does not already exist.
    /// </summary>
    /// <param name="key">The header key.</param>
    /// <param name="value">The header value.</param>
    /// <returns>The current <see cref="HttpRequest"/> instance.</returns>
    public HttpRequest AddHeader(string key, object? value)
    {
        Headers.TryAdd(key, value);
        return this;
    }

    /// <summary>
    /// Removes a header from the request.
    /// </summary>
    /// <param name="key">The header key.</param>
    /// <returns>The current <see cref="HttpRequest"/> instance.</returns>
    public HttpRequest RemoveHeader(string key)
    {
        Headers.Remove(key);
        return this;
    }

    /// <summary>
    /// Clears all headers from the request.
    /// </summary>
    /// <returns>The current <see cref="HttpRequest"/> instance.</returns>
    public HttpRequest ClearHeaders()
    {
        _headers = null;
        return this;
    }

    /// <summary>
    /// Sets the URL of the request.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The current <see cref="HttpRequest"/> instance.</returns>
    public HttpRequest WithUrl(IUrl url)
    {
        ArgumentNullException.ThrowIfNull(url);
        Url = url;
        return this;
    }

    /// <summary>
    /// Sets the HTTP method of the request.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <returns>The current <see cref="HttpRequest"/> instance.</returns>
    public HttpRequest WithMethod(HttpMethod method)
    {
        ArgumentNullException.ThrowIfNull(method);
        Method = method;
        return this;
    }

    /// <summary>
    /// Sets the content of the request.
    /// </summary>
    /// <param name="content">The HTTP content.</param>
    /// <returns>The current <see cref="HttpRequest"/> instance.</returns>
    public HttpRequest WithContent(HttpContent? content)
    {
        Content = content;
        return this;
    }
}