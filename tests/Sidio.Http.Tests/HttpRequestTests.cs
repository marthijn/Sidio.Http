namespace Sidio.Http.Tests;

public sealed partial class HttpRequestTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var method = HttpMethod.Get;
        var url = new Url().WithScheme("https").WithHost("example.com");
        var content = new StringContent("Test content");

        // Act
        var httpRequest = new HttpRequest(method, url, content);

        // Assert
        httpRequest.Method.Should().Be(method);
        httpRequest.Url.Should().Be(url);
        httpRequest.Content.Should().Be(content);
    }

    [Fact]
    public void IsValid_ValidRequest_ShouldReturnTrue()
    {
        // Arrange
        var method = HttpMethod.Get;
        var url = new Url().WithScheme("https").WithHost("example.com");
        var httpRequest = new HttpRequest(method, url);

        // Act
        var isValid = httpRequest.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, "example.com")]
    [InlineData("GET", null)]
    [InlineData(null, null)]
    public void IsValid_InvalidRequest_ShouldReturnFalse(string? method, string? host)
    {
        // Arrange
        var httpMethod = method != null ? new HttpMethod(method) : null;
        var url = host != null ? new Url().WithScheme("https").WithHost(host) : null;
        var httpRequest = new HttpRequest(httpMethod, url);

        // Act
        var isValid = httpRequest.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ValidRequest_ShouldReturnTrue()
    {
        // Arrange
        var method = HttpMethod.Get;
        var url = new Url().WithScheme("https").WithHost("example.com");
        var httpRequest = new HttpRequest(method, url);

        // Act
        Action act = httpRequest.Validate;

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(null, "example.com")]
    [InlineData("GET", null)]
    [InlineData(null, null)]
    public void Validate_InvalidRequest_ShouldThrowInvalidHttpRequestException(string? method, string? host)
    {
        // Arrange
        var httpMethod = method != null ? new HttpMethod(method) : null;
        var url = host != null ? new Url().WithScheme("https").WithHost(host) : null;
        var httpRequest = new HttpRequest(httpMethod, url);

        // Act
        Action act = httpRequest.Validate;

        // Assert
        act.Should().Throw<InvalidHttpRequestException>();
    }
}