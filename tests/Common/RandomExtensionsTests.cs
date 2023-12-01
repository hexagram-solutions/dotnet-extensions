namespace Hexagrams.Extensions.Common.Tests;

public class RandomExtensionsTests
{
    [Fact]
    public void Next_item_selects_item_in_collection()
    {
        var collection = new[] { "foo", "bar", "sut", "baz" };

        var item = Random.Shared.NextItem(collection);

        collection.Should().Contain(item);
    }

    [Fact]
    public void Next_item_throws_argument_exception_for_empty_collection()
    {
        var action = () => Random.Shared.NextItem(Array.Empty<string>());

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0, 5, 2)]
    [InlineData(-100, 0, 6)]
    [InlineData(0, 1, 1)]
    public void Next_decimal_produces_value_in_range(int minValue, int maxValue, byte scale)
    {
        var result = Random.Shared.NextDecimal(minValue, maxValue, scale);

        result.Should().BeGreaterOrEqualTo(minValue);
        result.Should().BeLessThanOrEqualTo(maxValue);
        result.Scale.Should().Be(scale);
    }

    [Fact]
    public void Next_decimal_throws_argument_exception_for_conflicting_min_and_max_values()
    {
        var action = () => Random.Shared.NextDecimal(0, 0, 0);

        action.Should().Throw<ArgumentException>();

        action = () => Random.Shared.NextDecimal(1, 0, 1);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Next_decimal_throws_argument_exception_if_precision_is_out_of_range()
    {
        var action = () => Random.Shared.NextDecimal(0, 1, 29);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}
