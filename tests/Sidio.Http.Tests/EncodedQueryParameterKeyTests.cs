namespace Sidio.Http.Tests;

public sealed class EncodedQueryParameterKeyTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void FromString_WithNullOrWhitespaceValue_ShouldThrowArgumentException(string? value)
    {
        // Act
        Action act = () => EncodedQueryParameterKey.FromString(value!, true);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("key", true, "key")]
    [InlineData("key", false, "key")]
    [InlineData(" value ", true, "%20value%20")]
    [InlineData(" value ", false, " value ")]
    [InlineData("val ue", true, "val%20ue")]
    [InlineData("val ue", false, "val ue")]
    public void FromString_WithValidValue_ShouldReturnEncodedQueryParameterKey(string value, bool encode, string expectedValue)
    {
        // Act
        var result = EncodedQueryParameterKey.FromString(value, encode);

        // Assert
        result.Value.Should().Be(expectedValue);
    }
}