namespace Sidio.Http.Tests;

public sealed partial class UrlTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesToDefaultValues()
    {
        // Arrange & Act
        var url = new Url();

        // Assert
        url.Scheme.Should().BeNull();
        url.Host.Should().BeNull();
        url.Port.Should().BeNull();
    }

    [Theory]
    [InlineData("https://example.com:8080", "https", "example.com", 8080)]
    [InlineData("http://example.com:80", "http", "example.com", null)]
    [InlineData("http://example.com", "http", "example.com", null)]
    public void From_WithUri_ReturnsUrlWithCorrectProperties(string input, string expectedScheme, string expectedHost, int? expectedPort)
    {
        // Arrange
        var uri = new Uri(input, UriKind.Absolute);

        // Act
        var url = Url.From(uri);

        // Assert
        url.Scheme.Should().Be(expectedScheme);
        url.Host.Should().Be(expectedHost);
        url.Port.Should().Be(expectedPort);
    }

    [Fact]
    public void From_WithUriWithPath_ReturnsUrlWithCorrectProperties()
    {
        // Arrange
        var uri = new Uri("https://example.com/path1/path2/path3/", UriKind.Absolute);

        // Act
        var url = Url.From(uri);

        // Assert
        url.Scheme.Should().Be("https");
        url.Host.Should().Be("example.com");
        url.Port.Should().BeNull();
        url.PathSegments.Should().BeEquivalentTo("path1", "path2", "path3");
    }
}