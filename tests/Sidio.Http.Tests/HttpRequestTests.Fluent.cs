namespace Sidio.Http.Tests;

public sealed partial class HttpRequestTests
{
    [Fact]
    public void WithHeaders_ShouldSetHeadersProperty()
    {
        // Arrange
        var httpRequest = new HttpRequest();
        var headers = new RequestHeaderCollection();
        headers.Add("Accept", "application/json");

        // Act
        var request = httpRequest.WithHeaders(headers);

        // Assert
        request.Should().BeSameAs(httpRequest);
        httpRequest.Headers.Should().BeEquivalentTo(headers);
    }

    [Fact]
    public void WithHeader_ShouldAddOrReplaceHeader()
    {
        // Arrange
        var httpRequest = new HttpRequest()
            .WithHeader("X-Test", "old");

        // Act
        var request = httpRequest.WithHeader("x-test", "new");

        // Assert
        request.Should().BeSameAs(httpRequest);
        httpRequest.Headers.TryGetValues("X-Test", out var values).Should().BeTrue();
        values.Should().Equal("new");
    }

    [Fact]
    public void AddHeader_ShouldOnlyAddWhenMissing()
    {
        // Arrange
        var httpRequest = new HttpRequest()
            .AddHeader("X-Test", "first");

        // Act
        var request = httpRequest.AddHeader("x-test", "second");

        // Assert
        request.Should().BeSameAs(httpRequest);
        httpRequest.Headers.TryGetValues("X-Test", out var values).Should().BeTrue();
        values.Should().Equal("first");
    }

    [Fact]
    public void RemoveHeader_ShouldRemoveHeader()
    {
        // Arrange
        var httpRequest = new HttpRequest()
            .WithHeader("X-Test", "value");

        // Act
        var request = httpRequest.RemoveHeader("x-test");

        // Assert
        request.Should().BeSameAs(httpRequest);
        httpRequest.Headers.ContainsKey("X-Test").Should().BeFalse();
    }

    [Fact]
    public void ClearHeaders_ShouldRemoveAllHeaders()
    {
        // Arrange
        var httpRequest = new HttpRequest()
            .WithHeader("X-Test", "value");

        // Act
        var request = httpRequest.ClearHeaders();

        // Assert
        request.Should().BeSameAs(httpRequest);
        httpRequest.Headers.Count.Should().Be(0);
    }

    [Fact]
    public void WithUrl_ShouldSetUrlProperty()
    {
        // Arrange
        var httpRequest = new HttpRequest();
        var expectedUrl = new Url().WithScheme("https").WithHost("example.com");

        // Act
        httpRequest.WithUrl(expectedUrl);

        // Assert
        httpRequest.Url.Should().Be(expectedUrl);
    }

    [Fact]
    public void WithMethod_ShouldSetMethodProperty()
    {
        // Arrange
        var httpRequest = new HttpRequest();
        var expectedMethod = HttpMethod.Post;

        // Act
        httpRequest.WithMethod(expectedMethod);

        // Assert
        httpRequest.Method.Should().Be(expectedMethod);
    }

    [Fact]
    public void WithContent_ShouldSetContentProperty()
    {
        // Arrange
        var httpRequest = new HttpRequest();
        var expectedContent = new StringContent("Test content");

        // Act
        httpRequest.WithContent(expectedContent);

        // Assert
        httpRequest.Content.Should().Be(expectedContent);
    }
}