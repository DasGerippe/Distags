using Serilog;

namespace SerilogUtils;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration ConfigureBootstrapLogger(
        this LoggerConfiguration loggerConfiguration, string outputTemplate = OutputTemplates.Default)
    {
        return loggerConfiguration.WriteTo.Console(outputTemplate: outputTemplate);
    }

    public static LoggerConfiguration ConfigureSinks<TProgram>(
        this LoggerConfiguration loggerConfiguration,
        bool writeToFile,
        string outputTemplate = OutputTemplates.Default)
    {
        loggerConfiguration.WriteTo.Console(outputTemplate: outputTemplate);

        if (writeToFile)
            loggerConfiguration.WriteTo.File(
                path: $"logs/{typeof(TProgram).Namespace}.txt",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 100_000_000,
                rollOnFileSizeLimit: true,
                outputTemplate: outputTemplate);

        return loggerConfiguration;
    }
}
