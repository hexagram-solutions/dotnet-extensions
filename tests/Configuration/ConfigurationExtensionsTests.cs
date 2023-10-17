using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Hexagrams.Extensions.Configuration.Tests;

public class ConfigurationExtensionsTests
{
    [Fact]
    public void Require_returns_configuration_value()
    {
        const string key = "foo";
        const string value = "bar";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { key, value } })
            .Build();

        config.Require(key).Should().Be(value);
    }

    [Fact]
    public void Require_throws_configuration_exception_when_key_is_not_found()
    {
        const string key = "foo";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { "bar", "baz" } })
            .Build();

        var test = () => config.Require(key);

        test.Should().Throw<ConfigurationException>().WithMessage($"*{key}*");
    }

    [Theory]
    [InlineData((string?) null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\r\n")]
    public void Require_throws_configuration_exception_when_value_is_null_or_white_space(string? badValue)
    {
        const string key = "foo";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { key, badValue } })
            .Build();

        var test = () => config.Require(key);

        test.Should().Throw<ConfigurationException>().WithMessage($"*{key}*");
    }

    [Fact]
    public void Require_T_returns_configuration_value_as_specified_type()
    {
        const string booleanKey = "booleanKey";
        const string intKey = "intKey";
        const string guidKey = "guidKey";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { booleanKey, true.ToString() },
                { intKey, 1.ToString(CultureInfo.InvariantCulture) },
                { guidKey, Guid.NewGuid().ToString() }
            })
            .Build();

        config.Require<bool>(booleanKey).Should().BeTrue();
        config.Require<int>(intKey).Should().Be(1);
        config.Require<Guid>(guidKey).Should().NotBeEmpty();
    }

    [Fact]
    public void Require_T_throws_exception_when_value_cannot_be_converted_to_type()
    {
        const string key = "foo";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { key, true.ToString() } })
            .Build();

        var test = () => config.Require<int>(key);

        test.Should().Throw<InvalidOperationException>().WithMessage($"*{key}*");
    }

    [Fact]
    public void Require_T_throws_exception_when_value_is_null()
    {
        const string key = "foo";

        var config = new ConfigurationBuilder()
            .Build();

        var test = () => config.Require<Guid>(key);

        test.Should().Throw<ConfigurationException>().WithMessage($"*{key}*");
    }

    [Fact]
    public void Collection_gets_configuration_section_values_as_array()
    {
        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(new
        {
            section = new
            {
                item1 = 1,
                item2 = 2,
                item3 = 3
            }
        });

        using var jsonStream = new MemoryStream(jsonBytes);

        var config = new ConfigurationBuilder()
            .AddJsonStream(jsonStream)
            .Build();

        var sectionValues = config.Collection("section");

        sectionValues.Should().BeEquivalentTo("1", "2", "3");
    }
}
