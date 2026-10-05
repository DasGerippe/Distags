using Serilog;

namespace SerilogUtils;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration ConfigureConsoleAndFileSinks(
        this LoggerConfiguration loggerConfiguration,
        string appName,
        string outputTemplate = OutputTemplates.Default)
    {
        return loggerConfiguration
            .WriteTo.Console(outputTemplate: outputTemplate)
            .WriteTo.File(
                path: $"logs/{appName}.txt",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 100_000_000,
                rollOnFileSizeLimit: true,
                outputTemplate: outputTemplate);
    }

    public static LoggerConfiguration ConfigureConsoleAndFileSinks<TProgram>(
        this LoggerConfiguration loggerConfiguration,
        string outputTemplate = OutputTemplates.Default)
    {
        return loggerConfiguration.ConfigureConsoleAndFileSinks(typeof(TProgram).Namespace!, outputTemplate);
    }
}
