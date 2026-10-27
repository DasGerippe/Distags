using Microsoft.Extensions.Logging;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterCsvRepositoryFactory
{
    private readonly ILoggerFactory _LoggerFactory;

    public MaxterCsvRepositoryFactory(ILoggerFactory loggerFactory)
    {
        _LoggerFactory = loggerFactory;
    }

    public MaxterCsvRepository CreateRepository(string csvPath)
    {
        return new MaxterCsvRepository(csvPath, _LoggerFactory.CreateLogger<MaxterCsvRepository>());
    }
}
