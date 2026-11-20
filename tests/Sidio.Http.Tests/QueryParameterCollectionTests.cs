namespace Sidio.Http.Tests;

public sealed class QueryParameterCollectionTests
{
    [Theory]
    [InlineData("key", true)]
    [InlineData("KEY", true)]
    [InlineData("nonexistent", false)]
    public void Parameters_ContainsKey_ShouldReturnExpectedResult(string key, bool expected)
    {
        // Arrange
        var sut = new QueryParameterCollection();
        sut.Add("key", "value");

        // Act
        var result = sut.Parameters.ContainsKey(key);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(QueryStringParameterTestDataWithExpectedValues))]
    public void Add_WithoutEncodingWithNullValueHandlingInclude_ShouldAddParameterToCollection(object? value, string expectedValue)
    {
        // Arrange
        var sut = new QueryParameterCollection();

        // Act
        sut.Add("key", value, false, NullValueHandling.Include);

        // Assert
        sut.Parameters.Should().ContainKey("key");
        sut.Parameters["key"].Should().ContainSingle().Which.Should().Be(expectedValue);
    }

    [Theory]
    [MemberData(nameof(QueryStringParameterTestDataWithEncodedExpectedValues))]
    public void Add_WithEncodingWithNullValueHandlingInclude_ShouldAddParameterToCollection(object? value, string expectedValue)
    {
        // Arrange
        var sut = new QueryParameterCollection();

        // Act
        sut.Add("key", value, true, NullValueHandling.Include);

        // Assert
        sut.Parameters.Should().ContainKey("key");
        sut.Parameters["key"].Should().ContainSingle().Which.Should().Be(expectedValue);
    }

    public static TheoryData<object?, string> QueryStringParameterTestDataWithExpectedValues => new()
    {
        { "value", "value" },
        { " value ", " value " },
        { "val ue", "val ue" },
        { 123, "123" },
        { true, "True" },
        { new DateTime(2024, 1, 1), "01/01/2024 00:00:00" },
        { null, string.Empty }
    };

    public static TheoryData<object?, string> QueryStringParameterTestDataWithEncodedExpectedValues => new()
    {
        { "value", "value" },
        { " value ", "%20value%20" },
        { "val ue", "val%20ue" },
        { 123, "123" },
        { true, "True" },
        { new DateTime(2024, 1, 1), "01%2F01%2F2024%2000%3A00%3A00" },
        { null, string.Empty }
    };
}