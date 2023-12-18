using Microsoft.Extensions.Configuration;

namespace Hexagrams.Extensions.Configuration;

/// <summary>
/// Extension methods for <see cref="IConfiguration"/>.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Get a configuration value, throwing an exception if the key is not found the or the value is whitespace.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="key">The configuration key.</param>
    /// <param name="exceptionMessage">A custom message for when no value is found for the configuration key.</param>
    /// <returns>The configuration value.</returns>
    /// <exception cref="ConfigurationException">The configuration value is not found or is whitespace.</exception>
    public static string Require(this IConfiguration config, string key, string? exceptionMessage = null)
    {
        var value = config[key];

        if (!string.IsNullOrWhiteSpace(value))
            return value;

        var message = exceptionMessage ?? $"No value found for configuration key {key}";

        throw new ConfigurationException(message);
    }

    /// <summary>
    /// Get a configuration value with the specified key and converts it to an instance of
    /// <typeparamref name="TValue"/>, throwing an exception if the key is not found or the value can't be converted.
    /// </summary>
    /// <typeparam name="TValue">The type to convert the value to.</typeparam>
    /// <param name="config">The configuration.</param>
    /// <param name="key">The configuration key.</param>
    /// <param name="exceptionMessage">A custom message for when no value is found for the configuration key.</param>
    /// <returns>The configuration value.</returns>
    /// <exception cref="ConfigurationException">
    /// The configuration value is not found or is not convertible to <typeparamref name="TValue"/>.
    /// </exception>
    public static TValue Require<TValue>(this IConfiguration config, string key, string? exceptionMessage = null)
    {
        var value = config.GetValue(typeof(TValue), key, null);

        if (value is TValue typedValue)
            return typedValue;

        var message = exceptionMessage ?? $"No value found for configuration key {key}";

        throw new ConfigurationException(message);
    }

    /// <summary>
    /// Get a configuration section's children as an array of values.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="section">The configuration section name.</param>
    /// <returns>The configuration values.</returns>
    public static string[] Collection(this IConfiguration config, string section)
    {
        return config.GetSection(section).GetChildren()
            .Select(x => x.Value!)
            .ToArray();
    }
}
