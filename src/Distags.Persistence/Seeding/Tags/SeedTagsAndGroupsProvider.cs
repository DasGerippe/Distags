using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Reflection;

namespace Distags.Persistence.Seeding.Tags;

public class SeedTagsAndGroupsProvider
{
    private readonly CsvConfiguration _CsvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = ";",
    };

    public List<SeedDisplayGroup> GetSeedDisplayGroups()
    {
        return GetEmbeddedCsvRecords<SeedDisplayGroup>("DisplayGroups.csv");
    }

    public List<SeedLogicGroup> GetSeedLogicGroups()
    {
        return GetEmbeddedCsvRecords<SeedLogicGroup>("LogicGroups.csv");
    }

    public List<SeedTag> GetSeedTags()
    {
        return GetEmbeddedCsvRecords<SeedTag>("Tags.csv");
    }

    private List<TCsvRecord> GetEmbeddedCsvRecords<TCsvRecord>(string csvFileName)
    {
        string csvFilePath = $"{nameof(Distags)}.{nameof(Persistence)}.Data.{csvFileName}";
        using Stream csvFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(csvFilePath)
                                     ?? throw new Exception($"Embedded resource {csvFilePath} not found.");
        using StreamReader streamReader = new StreamReader(csvFileStream);
        using CsvReader csvReader = new CsvReader(streamReader, _CsvConfig);

        return csvReader.GetRecords<TCsvRecord>().ToList();
    }
}
