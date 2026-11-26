namespace Sidio.Http.Tests;

public sealed class RequestHeaderCollectionTests
{
    [Fact]
    public void Add_WithNullValue_ShouldStoreEmptyValue()
    {
        // Arrange
        var sut = new RequestHeaderCollection();

        // Act
        sut.Add("X-Test", null);

        // Assert
        sut.Headers.Should().ContainKey("X-Test");
        sut.Headers["X-Test"].Should().Equal(string.Empty);
    }

    [Fact]
    public void AddOrReplace_WithTypedEnumerable_ShouldStoreEachItemAsSeparateValue()
    {
        // Arrange
        var sut = new RequestHeaderCollection();

        // Act
        sut.AddOrReplace("X-Ids", new[] { 1, 2, 3 });

        // Assert
        sut.Headers["X-Ids"].Should().Equal("1", "2", "3");
    }

    [Fact]
    public void TryAdd_WithExistingKey_ShouldReturnFalseAndKeepOriginalValue()
    {
        // Arrange
        var sut = new RequestHeaderCollection();
        sut.Add("X-Test", "first");

        // Act
        var result = sut.TryAdd("x-test", "second");

        // Assert
        result.Should().BeFalse();
        sut.Headers["X-Test"].Should().Equal("first");
    }

    [Theory]
    [InlineData("X-Test", true)]
    [InlineData("x-test", true)]
    [InlineData("missing", false)]
    public void ContainsKey_ShouldBeCaseInsensitive(string key, bool expected)
    {
        // Arrange
        var sut = new RequestHeaderCollection();
        sut.Add("X-Test", "value");

        // Act
        var result = sut.ContainsKey(key);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Clear_ShouldRemoveAllHeaders()
    {
        // Arrange
        var sut = new RequestHeaderCollection();
        sut.Add("A", "1");
        sut.Add("B", "2");

        // Act
        sut.Clear();

        // Assert
        sut.Count.Should().Be(0);
        sut.Headers.Should().BeEmpty();
    }
}
