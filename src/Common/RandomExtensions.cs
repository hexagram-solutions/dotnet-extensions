namespace Hexagrams.Extensions.Common;

/// <summary>
/// Extensions for <see cref="Random"/>.
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    /// Selects a random element in a collection.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <param name="random">The instance of <see cref="Random"/>.</param>
    /// <param name="items">The collection to select from.</param>
    /// <returns>A randomly selected element from the collection.</returns>
    /// <exception cref="ArgumentException">Thrown when the collection is empty.</exception>
    public static T NextItem<T>(this Random random, IEnumerable<T> items)
    {
        var enumerated = items as T[] ?? items.ToArray();

        if (enumerated.Length == 0)
            throw new ArgumentException("The collection contains no elements", nameof(items));

        return enumerated.ElementAt(random.Next(0, enumerated.Length));
    }

    /// <summary>
    /// Returns a random <see cref="decimal"/> within the specified range and with the specified scale.
    /// </summary>
    /// <param name="random">The instance of <see cref="Random"/>.</param>
    /// <param name="minValue">
    /// The inclusive lower bound of the random number returned. Must be less than <paramref name="maxValue"/>.
    /// </param>
    /// <param name="maxValue">
    /// The exclusive upper bound of the random number to be generated. Must be greater than
    /// <paramref name="minValue"/>.
    /// </param>
    /// <param name="scale">The number of decimal places to generate.</param>
    /// <returns>A random decimal value.</returns>
    public static decimal NextDecimal(this Random random, int minValue, int maxValue, byte scale)
    {
        if (minValue >= maxValue)
            throw new ArgumentException($"{nameof(minValue)} must be less than {nameof(maxValue)}");

        if (scale > 28)
            throw new ArgumentOutOfRangeException(nameof(scale), "Precision must be between 0 and 28");

        var scaleFactor = (decimal) Math.Pow(10, scale);

        var randomScale = random.Next((int) (minValue * scaleFactor) + 1, (int) (maxValue * scaleFactor) + 1);

        var randomDecimal = randomScale / scaleFactor;

        if (randomDecimal.Scale < scale)
        {
            // Hack to handle case where the decimal ends with a 0 and the scale is not preserved. For example, if
            // a decimal value of 1.1 is generated, but the scale is 2, this ensures that we return a decimal value
            // of 1.10.
            var zeroScalingValue = decimal.Parse($"0.{new string('0', scale)}");

            randomDecimal += zeroScalingValue;
        }

        return randomDecimal;
    }
}
