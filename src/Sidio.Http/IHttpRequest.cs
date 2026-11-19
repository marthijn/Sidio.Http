using System.Net.Http;

namespace Sidio.Http;

public interface IHttpRequest
{
    HttpMethod Method { get; init; }
}