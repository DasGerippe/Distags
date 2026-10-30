using Microsoft.Extensions.Configuration;
using System.Data.Common;

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

    /// <summary>
    /// Sets or replaces the password in a connection string to the value of a separate password config key.
    /// </summary>
    /// <remarks>
    /// If <c>"Default"</c> is passed as the <paramref name="connectionStringName"/>, the password in
    /// <c>ConnectionStrings:Default</c> will be replaced by the value in <c>Database:Default:Password</c>.<br/>
    /// If <c>Database:Default:Password</c> does not exist or its value is null, the connection string won't be modified.
    /// </remarks>
    /// <param name="configuration">The configuration in which the connection string is modified.</param>
    /// <param name="connectionStringName">THe name of the connection string that is modified.</param>
    public static void ReplaceConnectionStringPassword(this IConfiguration configuration, string connectionStringName)
    {
        string passwordConfigKey = $"Database:{connectionStringName}:Password";
        string? password = configuration[passwordConfigKey];
        if (password == null)
            return;

        string connectionString = configuration.GetRequiredConnectionString(connectionStringName);
        DbConnectionStringBuilder connectionStringBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString,
            ["Password"] = password,
        };
        configuration[$"ConnectionStrings:{connectionStringName}"] = connectionStringBuilder.ConnectionString;
    }
}
