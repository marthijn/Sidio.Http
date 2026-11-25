namespace Sidio.Http.Tests;

public sealed partial class HttpRequestTests
{
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