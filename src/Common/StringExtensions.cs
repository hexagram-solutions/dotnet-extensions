using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Hexagrams.Extensions.Common;

/// <summary>
/// Extension methods for working with <see cref="string" />s.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts a <see cref="string" /> to lowerCamelCase.
    /// </summary>
    /// <param name="value">The <see cref="string" /> to convert.</param>
    /// <returns>The converted <see cref="string" />.</returns>
    public static string ToLowerCamelCase(this string value)
    {
        return value.ToCamelCase();
    }

    /// <summary>
    /// Converts a <see cref="string" /> to UpperCamelCase.
    /// </summary>
    /// <param name="value">The <see cref="string" /> to convert.</param>
    /// <returns>The converted <see cref="string" />.</returns>
    public static string ToUpperCamelCase(this string value)
    {
        return value.ToCamelCase(true);
    }

    private static string ToCamelCase(this string value, bool upperCamelCase = false)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        // Split letters with diacritics into separate characters so that we can strip them away
        value = value.Normalize(NormalizationForm.FormD);

        var i = 0;
        var j = 0;

        var isStartOfWord = upperCamelCase;

        var result = new char[value.Length];

        // Skip any whitespace, punctuation, etc. in front of the first word...
        while (i < value.Length && !char.IsLetterOrDigit(value[i]))
            i++;

        for (; i < value.Length; i++)
        {
            // Anything that isn't a letter is the start of a new word...
            if (!char.IsLetter(value[i]))
            {
                // ... except in the edge case of the apostrophe in a contraction
                if ((value[i] == '\'' || value[i] == '\u2019') && 0 < i && i < value.Length - 1 &&
                    char.IsLetter(value[i - 1]) &&
                    char.IsLetter(value[i + 1]))
                {
                    continue;
                }

                // ... or a diacritic to be applied to another letter
                if (CharUnicodeInfo.GetUnicodeCategory(value[i]) == UnicodeCategory.NonSpacingMark)
                    continue;

                isStartOfWord = true;

                // We'll keep digits in the string...
                if (char.IsDigit(value[i]))
                    result[j++] = value[i];

                // But anything else that's both a non-letter and a non-digit gets skipped
                continue;
            }

            // We treat a translation from an uppercase to lowercase as the start of a new word. This handles the case
            // of applying ToCamelCase to a string that is already camel-cased.
            if (i > 0 && char.IsLower(value[i - 1]) && char.IsUpper(value[i]))
                isStartOfWord = true;

            if (isStartOfWord)
            {
                // start a new word
                result[j++] = char.ToUpperInvariant(value[i]);
                isStartOfWord = false;
            }
            else
            {
                // continue appending
                result[j++] = char.ToLowerInvariant(value[i]);
            }
        }

        return new string(result, 0, j);
    }

    /// <summary>
    /// Converts a <see cref="string" /> to kebab-case.
    /// </summary>
    /// <param name="value">The <see cref="string" /> to convert.</param>
    /// <returns>The converted <see cref="string" />.</returns>
    public static string ToKebabCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        // ToCamelCase normalizes strings for us, making them easier to split up
        var normalized = value.ToCamelCase();

        // Insert hyphens before uppercase letters, digits, and after digits
        var result = Regex.Replace(normalized, "(?<=[a-zA-Z])(?=[A-Z0-9])|(?<=[0-9])(?=[a-zA-Z])", "-");

        return result.ToLowerInvariant();
    }

    /// <summary>
    /// Converts a string to UPPER_SNAKE_CASE.
    /// </summary>
    /// <param name="value">The <see cref="string" /> to convert.</param>
    /// <returns>The converted <see cref="string" />.</returns>
    public static string ToUpperSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var result = ToSnakeCase(value);

        return result.ToUpperInvariant();
    }

    /// <summary>
    /// Converts a string to lower_snake_case.
    /// </summary>
    /// <param name="value">The <see cref="string" /> to convert.</param>
    /// <returns>The converted <see cref="string" />.</returns>
    public static string ToLowerSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var result = ToSnakeCase(value);

        return result.ToLowerInvariant();
    }

    private static string ToSnakeCase(string value)
    {
        // ToCamelCase normalizes strings for us, making them easier to split up
        var normalized = value.ToCamelCase();

        // Insert underscores before uppercase letters, digits, and after digits
        var result = Regex.Replace(normalized, "(?<=[a-zA-Z])(?=[A-Z0-9])|(?<=[0-9])(?=[a-zA-Z])", "_");

        return result;
    }

    /// <summary>
    /// Replace the diacritic characters in a string with their ASCII equivalents when possible. For example:
    /// <example>
    ///     <code>"Hafþór Júlíus Björnsson".StripDiacritics() == "Hafthor Julius Bjornsson"</code>
    /// </example>
    /// </summary>
    /// <param name="value">The value with diacritics.</param>
    /// <returns>The value without diacritics.</returns>
    /// <see href="http://stackoverflow.com/a/249126/1672990" />
    public static string StripDiacritics(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var normalizedString = value.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var character in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);

            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                stringBuilder.Append(character);
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Converts the string to a valid file name by replacing invalid chars with underscores or a given value.
    /// (e.g. <c>"08/03/2017".ToValidFileName() == "08_03_2017"</c>)
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="replacement">The string to replace invalid characters with.</param>
    /// <returns>A valid filename.</returns>
    public static string ToValidFileName(this string value, string replacement = "_")
    {
        var invalidChars = Path.GetInvalidFileNameChars();

        if (replacement.ToCharArray().Intersect(invalidChars).Any())
        {
            throw new ArgumentException($"{nameof(replacement)} cannot contain invalid file name characters.",
                nameof(replacement));
        }

        var result = new StringBuilder();

        foreach (var c in value.StripDiacritics())
        {
            if (c is >= ' ' and <= '~' && Array.IndexOf(invalidChars, c) < 0)
                result.Append(c);
            else
                result.Append(replacement);
        }

        return result.ToString();
    }
}
