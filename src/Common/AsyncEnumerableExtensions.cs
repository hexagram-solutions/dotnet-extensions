namespace Hexagrams.Extensions.Common;

/// <summary>
/// Extension methods for <see cref="IAsyncEnumerable{T}"/>.
/// </summary>
public static class AsyncEnumerableExtensions
{
    /// <summary>
    /// Enumerates all items in a <see cref="IAsyncEnumerable{T}"/> to a new <see cref="List{T}"/>.
    /// </summary>
    /// <param name="source">The <see cref="IAsyncEnumerable{T}"/></param>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <returns>The list of items.</returns>
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var items = await ExecuteAsync(source).ConfigureAwait(false);

        return items.ToList();
    }

    /// <summary>
    /// Enumerates all items in a <see cref="IAsyncEnumerable{T}"/> to a new array.
    /// </summary>
    /// <param name="source">The <see cref="IAsyncEnumerable{T}"/></param>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <returns>The array of items.</returns>
    public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var items = await ExecuteAsync(source).ConfigureAwait(false);

        return items.ToArray();
    }

    private static async Task<IEnumerable<T>> ExecuteAsync<T>(this IAsyncEnumerable<T> source)
    {
        var list = new List<T>();

        await foreach (var element in source)
            list.Add(element);

        return list;
    }
}
