using CsvHelper;
using CsvHelper.Configuration;
using Distags.Core.Commands.ImportOfficialLevels;
using System.Globalization;
using System.Reflection;

namespace Distags.Persistence.Seeding.OfficialLevels;

public class OfficialLevelsProvider
{
    public List<ScrapedOfficialLevel> GetOfficialLevels()
    {
        CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
        };

        const string officialLevelsCsvPath = $"{nameof(Distags)}.{nameof(Persistence)}.Data.OfficialLevels.csv";
        using Stream officialLevelsStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(officialLevelsCsvPath)
                                            ?? throw new Exception($"Embedded resource {officialLevelsCsvPath} not found.");
        using StreamReader officialLevelsReader = new StreamReader(officialLevelsStream);
        using CsvReader csvReader = new CsvReader(officialLevelsReader, csvConfig);

        csvReader.Context.RegisterClassMap<ScrapedOfficialLevelMap>();

        return csvReader.GetRecords<ScrapedOfficialLevel>().ToList();
    }
}
