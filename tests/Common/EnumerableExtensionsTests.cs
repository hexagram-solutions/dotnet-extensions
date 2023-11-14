namespace Hexagrams.Extensions.Common.Tests;

public class EnumerableExtensionsTests
{

    [Fact]
    public void Replace_replaces_first_matching_item_in_array()
    {
        const string itemToReplace = "baz";

        var items = new[] { "foo", itemToReplace, itemToReplace };

        const string newItem = "qux";

        var result = items.Replace(itemToReplace, newItem);

        result.Should().BeEquivalentTo(new[] { "foo", newItem, itemToReplace }, opts => opts.WithStrictOrdering());
    }

    [Fact]
    public void Replace_throws_exception_when_item_is_not_found()
    {
        var items = new[] { "foo", "bar", "baz" };

        var action = () => items.Replace("qux", "garply");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Replace_replaces_complex_objects()
    {
        var item1 = new Foo("qux", 1);
        var item2 = new Foo("garply", 2);

        var items = new List<Foo> { item1, item2 };

        var newItem = new Foo("wumbo", 3);

        var result = items.Replace(item1, newItem);

        result.Should().BeEquivalentTo(new[] { newItem, item2 }, opts => opts.WithStrictOrdering());
    }

    private record Foo(string Bar, int Baz);
}
