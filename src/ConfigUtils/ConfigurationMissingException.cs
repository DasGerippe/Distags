namespace ConfigUtils;

public class ConfigurationMissingException : Exception
{
    public ConfigurationMissingException(string configKey)
        : base($"Configuration key '{configKey}' is missing or its value is null.")
    {
    }
}
