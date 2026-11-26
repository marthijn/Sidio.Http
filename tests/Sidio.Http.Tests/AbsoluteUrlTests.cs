namespace Sidio.Http.Tests;

public sealed class AbsoluteUrlTests
{
    [Fact]
    public void ToUri_ShouldCreateAbsoluteUri()
    {
        // Arrange
        var url = new AbsoluteUrl(Url.From("https://example.com:8443/path?q=one&q=two"));

        // Act
        var uri = url.ToUri();

        // Assert
        uri.IsAbsoluteUri.Should().BeTrue();
        uri.AbsoluteUri.Should().Be("https://example.com:8443/path?q=one&q=two");
    }

    [Fact]
    public void Constructor_WhenInvalidUrl_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => _ = new AbsoluteUrl(Url.From("invalid-url"));

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ImplicitOperator_FromAbsoluteUrlToUri_ShouldCreateAbsoluteUri()
    {
        // Arrange
        var url = new AbsoluteUrl(Url.From("https://example.com:8443/path?q=one&q=two"));

        // Act
        Uri uri = url;

        // Assert
        uri.IsAbsoluteUri.Should().BeTrue();
        uri.AbsoluteUri.Should().Be("https://example.com:8443/path?q=one&q=two");
    }
}