namespace Hexagrams.Extensions.Common.Tests;

public class AsyncEnumerableExtensionsTests
{
    [Fact]
    public async Task ToListAsync_enumerates_items_from_async_collection()
    {
        const int start = 0;
        const int end = 5;

        var enumeratedItems = await RangeAsync(start, end).ToListAsync();

        var expectedItems = Enumerable.Range(start, end);

        enumeratedItems.Should().BeEquivalentTo(expectedItems);
    }

    [Fact]
    public async Task ToListAsync_throws_exception_when_source_is_null()
    {
        var action = () => ((IAsyncEnumerable<int>) null!).ToListAsync();

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ToArrayAsync_enumerates_items_from_async_collection()
    {
        const int start = 0;
        const int end = 5;

        var enumeratedItems = await RangeAsync(start, end).ToArrayAsync();

        var expectedItems = Enumerable.Range(start, end);

        enumeratedItems.Should().BeEquivalentTo(expectedItems);
    }

    [Fact]
    public async Task ToArrayAsync_throws_exception_when_source_is_null()
    {
        var action = () => ((IAsyncEnumerable<int>) null!).ToArrayAsync();

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    private static async IAsyncEnumerable<int> RangeAsync(int start, int count)
    {
        for (var i = 0; i < count; i++)
        {
            await Task.Delay(100);
            yield return start + i;
        }
    }
}
