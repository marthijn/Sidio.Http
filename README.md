# Sidio.Http

[![build](https://github.com/marthijn/Sidio.Http/actions/workflows/build.yml/badge.svg)](https://github.com/marthijn/Sidio.Http/actions/workflows/build.yml)
[![NuGet Version](https://img.shields.io/nuget/v/Sidio.Http)](https://www.nuget.org/packages/Sidio.Http/)

# Features
## Fluent URL builder

`Url` provides a fluent API for constructing and parsing HTTP and HTTPS URLs.

```csharp
var url = new Url()
    .WithScheme("https")
    .WithHost("example.com")
    .WithPort(8443)
    .AppendPath("orders", "order details")
    .WithQueryParameter("status", "open")
    .WithQueryParameter("tag", new[] { "new", "priority" });

Console.WriteLine(url);
// https://example.com:8443/orders/order%20details?status=open&tag=new&tag=priority
```

Fluent methods mutate the URL and return the same instance.

### Parse a URL

```csharp
var url = Url.From("https://example.com:8080/search?q=one&q=two");

url.Scheme;  // "https"
url.Host;    // "example.com"
url.Port;    // 8080
url.PathSegments; // ["search"]
```

`Url.From(Uri)` is also available.

### Work with paths

```csharp
var url = new Url()
    .AppendPath("api", "users")
    .AppendPath("active");

url.SetPath("api", "teams");
url.ClearPath();
```

Path segments are separated with `/` and escaped when the URL is rendered.

### Work with query parameters

```csharp
var url = new Url()
    .WithQueryParameter("page", 2)
    .AddQueryParameter("filter", "active")
    .RemoveQueryParameter("filter");

url.QueryParameters.Count;
url.QueryParameters.ContainsKey("page");

if (url.QueryParameters.TryGetValues("page", out var pages))
{
    // pages contains "2"
}

url.ClearQueryParameters();
```

- `WithQueryParameter` adds or replaces a parameter.
- `AddQueryParameter` only adds a parameter when its key is not already present.
- `RemoveQueryParameter` removes a parameter by key.
- `ClearQueryParameters` removes all query parameters.

For direct query collection access, use `QueryParameters`. It supports case-insensitive key lookup and repeated values.

### Ensure an absolute URL

```csharp
var url = new Url("https://example.com/api");
var absoluteUrl = url.ToAbsoluteUrl();

var relativeUrl = new Url("/api");
_ = relativeUrl.ToAbsoluteUrl(); // throws InvalidOperationException
```

## Fluent HTTP Client
To be added in the future.

## HTTP testing framework
To be added in the future.

# Background
This library is inspired by [Flurl](https://flurl.dev/), which is unfortunately no longer maintained. 
The goal of this library is to provide a simple, fluent, and testable HTTP client for .NET applications.