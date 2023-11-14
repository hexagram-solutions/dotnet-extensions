namespace Hexagrams.Extensions.Common;

/// <summary>
/// Extension methods for object collections.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Replaces the first matching item in a collection with a new value using the default equality comparer for the
    /// type.
    /// </summary>
    /// <typeparam name="TItem">The type of objects in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <param name="oldValue">The value to replace.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="comparer">
    /// The equality comparer to use. Defaults to the default equality comparer for the type.
    /// </param>
    /// <exception cref="ArgumentException">The item could not be found in the collection.</exception>
    /// <returns>A new collection with the replaced result.</returns>
    /// <remarks>This method will enumerate the items in the source collection.</remarks>
    /// <exception cref="ArgumentException"></exception>
    public static IEnumerable<TItem> Replace<TItem>(this IEnumerable<TItem> source, TItem oldValue, TItem newValue,
        IEqualityComparer<TItem>? comparer = null)
    {
        var array = source as TItem[] ?? source.ToArray();

        comparer ??= EqualityComparer<TItem>.Default;

        var found = array
            .Select((item, index) => new { item, index })
            .FirstOrDefault(x => comparer.Equals(x.item, oldValue));

        var index = found?.index ?? -1;

        if (index == -1)
        {
            throw new ArgumentException("No matching value was found in the collection using the specified equality " +
                "comparer.", nameof(oldValue));
        }

        array[index] = newValue;

        return array.ToArray();
    }
}
