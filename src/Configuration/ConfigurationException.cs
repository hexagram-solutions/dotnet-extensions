namespace Hexagrams.Extensions.Configuration;

/// <summary>
/// Represents errors that occur during application configuration.
/// </summary>
/// <param name="message">The configuration error message.</param>
public class ConfigurationException(string message) : Exception(message)
{
}
