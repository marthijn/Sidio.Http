namespace Sidio.Http.Tests;

public sealed class QueryParameterKeyComparerTests
{
    [Theory]
    [InlineData("key", "key", true)]
    [InlineData("key", "KEY", true)]
    [InlineData("key", "differentKey", false)]
    public void Equals_ShouldReturnExpectedResult(string x, string y, bool expected)
    {
        // Arrange
        var comparer = new QueryParameterKeyComparer();
        var encodedX = EncodedQueryParameterKey.FromString(x, true);
        var encodedY = EncodedQueryParameterKey.FromString(y, true);

        // Act
        var result = comparer.Equals(encodedX, encodedY);

        // Assert
        result.Should().Be(expected);
    }
}