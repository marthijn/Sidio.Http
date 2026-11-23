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

    [Fact]
    public void From_WithUriWithQuery_MergesDuplicateQueryValues()
    {
        // Arrange
        var uri = new Uri("https://example.com/search?q=one&q=two", UriKind.Absolute);

        // Act
        var url = Url.From(uri);

        // Assert
        url.QueryParameters.TryGetValues("q", out var values).Should().BeTrue();
        values.Should().Equal("one", "two");
    }

    [Fact]
    public void ToString_WithComponents_ShouldBuildUrl()
    {
        // Arrange
        var url = new Url()
            .WithScheme("https")
            .WithHost("example.com")
            .WithPort(8443)
            .AppendPath("path 1", "path2")
            .WithQueryParameter("q", "hello world")
            .WithQueryParameter("tag", new[] { "one", "two" });

        // Act
        var result = url.ToString();

        // Assert
        result.Should().Be("https://example.com:8443/path%201/path2?q=hello%20world&tag=one&tag=two");
    }

    [Fact]
    public void SetPath_ShouldReplaceExistingPath()
    {
        // Arrange
        var url = new Url().AppendPath("old", "path");

        // Act
        var url2 = url.SetPath("/new/path/");

        // Assert
        url2.Should().BeSameAs(url);
        url.PathSegments.Should().BeEquivalentTo("new", "path");
    }

    [Fact]
    public void ClearQueryParameters_ShouldRemoveQueryString()
    {
        // Arrange
        var url = Url.From("https://example.com/path?a=1");

        // Act
        var url2 = url.ClearQueryParameters();

        // Assert
        url2.Should().BeSameAs(url);
        url.ToString().Should().Be("https://example.com/path");
    }

    [Theory]
    [InlineData("https://example.com/path?a=1", true)]
    [InlineData("http://example.com/path", true)]
    [InlineData("example.com/path", false)]
    [InlineData("/relative/path", false)]
    [InlineData("/relative/path?a=b", false)]
    public void IsAbsolute_ShouldReturnExpected(string url, bool expected)
    {
        // Arrange
        var sut = Url.From(url);

        // Act
        var result = sut.IsAbsolute;

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("https://localhost/relative/path", "https://localhost/relative/path")]
    [InlineData("http://localhost/relative/path", "http://localhost/relative/path")]
    public void ToAbsoluteUrl_ShouldReturnAbsoluteUrl(string url, string expected)
    {
        // Arrange
        var sut = Url.From(url);

        // Act
        var result = sut.ToAbsoluteUrl();

        // Assert
        result.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("/relative/path")]
    [InlineData("//localhost/relative/path")]
    public void ToAbsoluteUrl_WhenUrlIsNotAbsolute_ShouldThrowInvalidOperationException(string url)
    {
        // Arrange
        var sut = Url.From(url);

        // Act
        Action act = () => sut.ToAbsoluteUrl();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitOperator_FromUrlToUri_ShouldCreateAbsoluteUri()
    {
        // Arrange
        var url = Url.From("https://example.com:8443/path?q=one&q=two");

        // Act
        Uri uri = url;

        // Assert
        uri.IsAbsoluteUri.Should().BeTrue();
        uri.AbsoluteUri.Should().Be("https://example.com:8443/path?q=one&q=two");
    }

    [Fact]
    public void ImplicitOperator_FromUrlToUri_ShouldCreateRelativeUri()
    {
        // Arrange
        var url = Url.From("/path?q=one");

        // Act
        Uri uri = url;

        // Assert
        uri.IsAbsoluteUri.Should().BeFalse();
        uri.OriginalString.Should().Be("/path?q=one");
    }

    [Fact]
    public void ImplicitOperator_FromUriToUrl_ShouldCreateAbsoluteUrl()
    {
        // Arrange
        var uri = new Uri("https://example.com:8443/path?q=one&q=two", UriKind.Absolute);

        // Act
        Url url = uri;

        // Assert
        url.ToString().Should().Be("https://example.com:8443/path?q=one&q=two");
        url.IsAbsolute.Should().BeTrue();
    }

    [Fact]
    public void ImplicitOperator_FromUriToUrl_ShouldCreateRelativeUrl()
    {
        // Arrange
        var uri = new Uri("/path?q=one", UriKind.Relative);

        // Act
        Url url = uri;

        // Assert
        url.ToString().Should().Be("/path?q=one");
        url.IsAbsolute.Should().BeFalse();
    }

    [Fact]
    public void ImplicitOperators_ShouldRoundTripAbsoluteUrl()
    {
        // Arrange
        var original = Url.From("https://example.com/search?q=one&q=two");

        // Act
        Uri uri = original;
        Url roundTripped = uri;

        // Assert
        roundTripped.Scheme.Should().Be("https");
        roundTripped.Host.Should().Be("example.com");
        roundTripped.PathSegments.Should().Equal("search");
        roundTripped.QueryParameters.TryGetValues("q", out var values).Should().BeTrue();
        values.Should().Equal("one", "two");
    }
}