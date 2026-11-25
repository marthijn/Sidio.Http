namespace Sidio.Http;

public sealed partial class HttpRequest
{
    public HttpRequest WithUrl(IUrl url)
    {
        ArgumentNullException.ThrowIfNull(url);
        Url = url;
        return this;
    }

    public HttpRequest WithMethod(HttpMethod method)
    {
        ArgumentNullException.ThrowIfNull(method);
        Method = method;
        return this;
    }

    public HttpRequest WithContent(HttpContent? content)
    {
        Content = content;
        return this;
    }
}