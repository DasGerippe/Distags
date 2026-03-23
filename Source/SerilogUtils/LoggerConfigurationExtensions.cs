using Serilog;

namespace SerilogUtils;

public static class LoggerConfigurationExtensions
{
    private const string DefaultOutputTemplate =
        "{Timestamp:HH:mm:ss.fff} [{Level}] [{SourceContext}]{NewLine}{Message:lj}{NewLine}{Exception}";

    public static LoggerConfiguration ConfigureConsoleAndFileSinks(this LoggerConfiguration loggerConfiguration, string appName)
    {
        return loggerConfiguration
            .WriteTo.Console(outputTemplate: DefaultOutputTemplate)
            .WriteTo.File(
                path: $"logs/{appName}.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: DefaultOutputTemplate);
    }
}