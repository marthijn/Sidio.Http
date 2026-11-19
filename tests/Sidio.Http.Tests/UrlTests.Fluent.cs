namespace Sidio.Http.Tests;

public sealed partial class UrlTests
{
    [Fact]
    public void WithPort_ShouldSetPortProperty()
    {
        // Arrange
        var url = new Url();
        var expectedPort = Random.Shared.Next(1, 65535);

        // Act
        url.WithPort(expectedPort);

        // Assert
        url.Port.Should().Be(expectedPort);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(65536)]
    public void WithPort_WhenOutOfRange_ShouldThrowArgumentOutOfRangeException(int invalidPort)
    {
        // Arrange
        var url = new Url();

        // Act
        Action act = () => url.WithPort(invalidPort);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(80, "http")]
    [InlineData(443, "https")]
    public void WithPort_WhenDefaultPortForScheme_ShouldSetPortToNull(int port, string scheme)
    {
        // Arrange
        var url = new Url().WithScheme(scheme);

        // Act
        url.WithPort(port);

        // Assert
        url.Port.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("http")]
    [InlineData("https")]
    [InlineData("http ")]
    [InlineData(" https")]
    [InlineData("HTTP")]
    [InlineData("HtTpS")]
    public void WithScheme_ShouldSetSchemeProperty(string? validScheme)
    {
        // Arrange
        var url = new Url();

        // Act
        url.WithScheme(validScheme);

        // Assert
        url.Scheme.Should().Be(validScheme?.Trim());
    }

    [Theory]
    [InlineData("ftp")]
    [InlineData("file")]
    [InlineData("mailto")]
    [InlineData("h t t p")]
    public void WithScheme_WhenInvalidScheme_ShouldThrowArgumentException(string invalidScheme)
    {
        // Arrange
        var url = new Url();

        // Act
        Action act = () => url.WithScheme(invalidScheme);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("example.com", "example.com")]
    [InlineData("//example.com", "example.com")]
    [InlineData("http://example.com", "example.com")]
    [InlineData("https://example.com", "example.com")]
    [InlineData("https://example.com/path", "example.com")]
    [InlineData("example.com:81", "example.com")]
    [InlineData("subdomain.example.com", "subdomain.example.com")]
    [InlineData("ExAmPlE.CoM", "example.com")]
    [InlineData("localhost", "localhost")]
    [InlineData("127.0.0.1", "127.0.0.1")]
    [InlineData("[::1]", "[::1]")]
    public void WithHost_ShouldSetHostProperty(string? host, string? expectedHost)
    {
        // Arrange
        var url = new Url();

        // Act
        url.WithHost(host);

        // Assert
        url.Host.Should().Be(expectedHost);
    }

    [Fact]
    public void AppendPath_ShouldAddPathSegments()
    {
        // Arrange
        var url = new Url();

        // Act
        url.AppendPath("segment1", "segment2", "segment3");

        // Assert
        url.PathSegments.Should().BeEquivalentTo("segment1", "segment2", "segment3");
    }
}