using Microsoft.Extensions.Configuration;

namespace ConfigUtils;

public static class ConfigurationExtensions
{
    public static string GetRequiredValue(this IConfiguration configuration, string key)
    {
        return configuration[key] ?? throw new ConfigurationMissingException(key);
    }

    public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
    {
        return configuration.GetConnectionString(name) ?? throw new ConfigurationMissingException(name);
    }
}
