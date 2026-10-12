namespace SerilogUtils;

public static class OutputTemplates
{
    public const string Default =
        "{Timestamp:HH:mm:ss.fff} [{Level}] [{SourceContext}]{NewLine}{Message:lj}{NewLine}{Exception}";

    public const string Web =
        "{Timestamp:HH:mm:ss.fff} [{Level}] [{SourceContext}] {TraceId}{NewLine}{Message:lj}{NewLine}{Exception}";
}
