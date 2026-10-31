using Microsoft.Extensions.Configuration;

namespace AppHostUtils;

public static class ConfigurationBuilderExtensions
{
    public static void AddDockerSecrets(this IConfigurationBuilder configuration)
    {
        configuration.AddKeyPerFile("/run/secrets", optional: true);
    }
}
