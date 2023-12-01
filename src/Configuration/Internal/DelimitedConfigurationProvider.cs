using Microsoft.Extensions.Configuration;

namespace Hexagrams.Extensions.Configuration.Internal;

internal sealed class DelimitedConfigurationProvider(IEnumerable<string> keyDelimiters, IConfiguration config)
    : ConfigurationProvider
{
    public override void Load()
    {
        var existingItems = new Dictionary<string, string>(config.AsEnumerable()!);

        foreach (var setting in existingItems)
        {
            var normalizedKeysWithValues = NormalizeKey(setting.Key)
                .Where(normalizedKey => !Data.ContainsKey(normalizedKey));

            foreach (var normalizedKey in normalizedKeysWithValues)
                Data.Add(normalizedKey, setting.Value);
        }
    }

    private IEnumerable<string> NormalizeKey(string key)
    {
        var newKeys = new List<string>();

        foreach (var delimiter in keyDelimiters)
            newKeys.AddRange(keyDelimiters.Select(d => key.Replace(delimiter, d)).Distinct());

        return newKeys.AsEnumerable();
    }
}
