// ReSharper disable StringLiteralTypo
namespace Hexagrams.Extensions.Common.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void To_camel_case_has_no_effect_on_null_or_empty_strings()
    {
        StringExtensions.ToUpperCamelCase(null!).Should().BeNull();
        StringExtensions.ToLowerCamelCase(null!).Should().BeNull();

        string.Empty.ToUpperCamelCase().Should().BeEmpty();
        string.Empty.ToLowerCamelCase().Should().BeEmpty();
    }

    [Theory]
    [InlineData("foo", "foo", "Foo")]
    [InlineData("foo bar baz", "fooBarBaz", "FooBarBaz")]
    [InlineData("FOO bar BAZ", "fooBarBaz", "FooBarBaz")]
    [InlineData("fooBar BAZ", "fooBarBaz", "FooBarBaz")]
    [InlineData("alreadyCamelCase", "alreadyCamelCase", "AlreadyCamelCase")]
    [InlineData("AlreadyCamelCase", "alreadyCamelCase", "AlreadyCamelCase")]
    [InlineData("Words like crème brûlée have accents removed ", "wordsLikeCremeBruleeHaveAccentsRemoved", "WordsLikeCremeBruleeHaveAccentsRemoved")]
    [InlineData("Numbers like 314 become part of the string", "numbersLike314BecomePartOfTheString", "NumbersLike314BecomePartOfTheString")]
    [InlineData("9/11 changed the world", "911ChangedTheWorld", "911ChangedTheWorld")]
    [InlineData("   \t\r\n  white space is removed from the start", "whiteSpaceIsRemovedFromTheStart", "WhiteSpaceIsRemovedFromTheStart")]
    [InlineData("and the end\r\n", "andTheEnd", "AndTheEnd")]
    [InlineData("\t or both ", "orBoth", "OrBoth")]
    [InlineData("     ", "", "")] // or entirely
    [InlineData("'puncuation\"usually\"starts a [new] word'.", "puncuationUsuallyStartsANewWord", "PuncuationUsuallyStartsANewWord")]
    [InlineData("but it can't when it's a contraction of two words.", "butItCantWhenItsAContractionOfTwoWords", "ButItCantWhenItsAContractionOfTwoWords")]
    [InlineData("ToCamelCase_converts_any_string_to_camelCase", "toCamelCaseConvertsAnyStringToCamelCase", "ToCamelCaseConvertsAnyStringToCamelCase")]
    public void To_camel_case_converts_strings_to_camel_case(
        string input, string expectedLowerCamelCase, string expectedUpperCamelCase)
    {
        input.ToLowerCamelCase().Should().Be(expectedLowerCamelCase);
        input.ToUpperCamelCase().Should().Be(expectedUpperCamelCase);
    }

    [Fact]
    public void To_kebab_case_has_no_effect_on_null_or_empty_strings()
    {
        StringExtensions.ToKebabCase(null!).Should().BeNull();

        string.Empty.ToKebabCase().Should().BeEmpty();
    }

    [Theory]
    [InlineData("foo", "foo")]
    [InlineData("foo bar baz", "foo-bar-baz")]
    [InlineData("FOO bar BAZ", "foo-bar-baz")]
    [InlineData("fooBar BAZ", "foo-bar-baz")]
    [InlineData("already-kebab-case", "already-kebab-case")]
    [InlineData("Already-kebab-case", "already-kebab-case")]
    [InlineData("Words like crème brûlée have accents removed ", "words-like-creme-brulee-have-accents-removed")]
    [InlineData("Numbers like 314 become part of the string", "numbers-like-314-become-part-of-the-string")]
    [InlineData("9/11 changed the world", "911-changed-the-world")]
    [InlineData("   \t\r\n  white space is removed from the start", "white-space-is-removed-from-the-start")]
    [InlineData("and the end\r\n", "and-the-end")]
    [InlineData("\t or both ", "or-both")]
    [InlineData("     ", "")] // or entirely
    [InlineData("'puncuation\"usually\"starts a [new] word'.", "puncuation-usually-starts-a-new-word")]
    [InlineData("but it can't when it's a contraction of two words.", "but-it-cant-when-its-a-contraction-of-two-words")]
    [InlineData("ToKebabCase_converts_any_string_to_kebab_Case", "to-kebab-case-converts-any-string-to-kebab-case")]
    public void To_kebab_case_converts_strings_to_kebab_case(string input, string expected)
    {
        input.ToKebabCase().Should().Be(expected);
    }

    [Theory]
    [InlineData("foo", "foo")]
    [InlineData("foo bar baz", "foo_bar_baz")]
    [InlineData("FOO bar BAZ", "foo_bar_baz")]
    [InlineData("fooBar BAZ", "foo_bar_baz")]
    [InlineData("already_kebab_case", "already_kebab_case")]
    [InlineData("Already_kebab_case", "already_kebab_case")]
    [InlineData("Words like crème brûlée have accents removed ", "words_like_creme_brulee_have_accents_removed")]
    [InlineData("Numbers like 314 become part of the string", "numbers_like_314_become_part_of_the_string")]
    [InlineData("9/11 changed the world", "911_changed_the_world")]
    [InlineData("   \t\r\n  white space is removed from the start", "white_space_is_removed_from_the_start")]
    [InlineData("and the end\r\n", "and_the_end")]
    [InlineData("\t or both ", "or_both")]
    [InlineData("     ", "")] // or entirely
    [InlineData("'puncuation\"usually\"starts a [new] word'.", "puncuation_usually_starts_a_new_word")]
    [InlineData("but it can't when it's a contraction of two words.", "but_it_cant_when_its_a_contraction_of_two_words")]
    [InlineData("ToKebabCase_converts_any_string_to_kebab_Case", "to_kebab_case_converts_any_string_to_kebab_case")]
    public void To_snake_case_converts_strings_to_snake_case(string input, string expected)
    {
        input.ToLowerSnakeCase().Should().Be(expected);
        input.ToUpperSnakeCase().Should().Be(expected.ToUpperInvariant());
    }

    [Theory]
    [InlineData("Éric Söndergard", "Eric Sondergard")]
    [InlineData("Gêorgé Costanzà", "George Costanza")]
    [InlineData("Hafthór Júlíus Björnsson", "Hafthor Julius Bjornsson")]
    public static void StripDiacritics_normalizes_strings_with_diacritics(string input, string expected)
    {
        input.StripDiacritics().Should().Be(expected);
    }

    [Theory]
    [InlineData("Eric Sondergard", "Eric Sondergard")]
    [InlineData("George Costanza", "George Costanza")]
    [InlineData("Hafthor Julíus Bjornsson", "Hafthor Julius Bjornsson")]
    public static void StripDiacritics_does_not_modify_strings_with_no_diacritics(string input, string expected)
    {
        input.StripDiacritics().Should().Be(expected);
    }

    [Theory]
    [InlineData("24/05/2017")]
    [InlineData(@"24\05\2017")]
    [InlineData("24>05<2017:24|05?*2017")]
    [InlineData("Fo\"o")]
    public void ToValidFileName_converts_value_to_valid_filename(string value)
    {
        var invalidCharactersInResult = value.ToValidFileName()
            .ToCharArray()
            .Intersect(Path.GetInvalidFileNameChars());

        invalidCharactersInResult.Should().BeEmpty();
    }

    public static IEnumerable<object[]> InvalidFileNameChars =>
        Path.GetInvalidFileNameChars().Select(c => new object[] { c });

    [Theory]
    [MemberData(nameof(InvalidFileNameChars))]
    public void ToValidFileName_throws_ArgumentException_if_replacement_string_contains_invalid_chars(char invalid)
    {
        const string value = "17/05/2017";

        var action = () => value.ToValidFileName(invalid.ToString());

        action.Should().Throw<ArgumentException>();
    }
}
