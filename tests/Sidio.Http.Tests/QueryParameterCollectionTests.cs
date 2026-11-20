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

    [Fact]
    public void Count_ShouldReflectCurrentNumberOfKeys()
    {
        // Arrange
        var sut = new QueryParameterCollection();

        // Act
        sut.Add("a", "1");
        sut.Add("b", "2");
        sut.AddOrReplace("a", "3");

        // Assert
        sut.Count.Should().Be(2);
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

    [Fact]
    public void Add_WithTypedEnumerable_ShouldAddEachItemAsSeparateValue()
    {
        // Arrange
        var sut = new QueryParameterCollection();

        // Act
        sut.Add("numbers", new[] { 1, 2, 3 }, false, NullValueHandling.Include);

        // Assert
        sut.Parameters.Should().ContainKey("numbers");
        sut.Parameters["numbers"].Should().Equal("1", "2", "3");
    }

    [Fact]
    public void Constructor_WithDuplicateKeys_ShouldMergeValues()
    {
        // Act
        var sut = new QueryParameterCollection("?a=1&a=2&a=three");

        // Assert
        sut.Parameters.Should().ContainKey("a");
        sut.Parameters["a"].Should().Equal("1", "2", "three");
    }

    [Fact]
    public void TryGetValues_WithExistingKey_ShouldReturnReadOnlyValues()
    {
        // Arrange
        var sut = new QueryParameterCollection("?key=one&key=two");

        // Act
        var result = sut.TryGetValues("key", out var values);

        // Assert
        result.Should().BeTrue();
        values.Should().Equal("one", "two");
        values.Should().NotBeAssignableTo<List<string>>();
    }

    [Theory]
    [InlineData("key", true)]
    [InlineData("KEY", true)]
    [InlineData("missing", false)]
    public void ContainsKey_ShouldReturnExpectedResult(string key, bool expected)
    {
        // Arrange
        var sut = new QueryParameterCollection();
        sut.Add("key", "value");

        // Act
        var result = sut.ContainsKey(key);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToQueryString_ShouldSerializeMultipleValues()
    {
        // Arrange
        var sut = new QueryParameterCollection();
        sut.Add("a", "1");
        sut.Add("b", new[] { "x", "y" });

        // Act
        var result = sut.ToQueryString();

        // Assert
        result.Should().Be("?a=1&b=x&b=y");
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