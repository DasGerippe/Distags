using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterCsvRepository
{
    private readonly string _CsvPath;
    private readonly Lazy<IReadOnlyDictionary<long, MaxterCsvRow>> _Rows;

    public MaxterCsvRepository(string csvPath)
    {
        _CsvPath = csvPath;
        _Rows = new Lazy<IReadOnlyDictionary<long, MaxterCsvRow>>(LoadRows);
    }

    public MaxterCsvRow? FindRow(long levelId)
    {
        return _Rows.Value.GetValueOrDefault(levelId);
    }

    public IReadOnlyCollection<MaxterCsvRow> GetAllRows()
    {
        return _Rows.Value.Values.ToList();
    }

    private IReadOnlyDictionary<long, MaxterCsvRow> LoadRows()
    {
        if (!File.Exists(_CsvPath))
        {
            Console.WriteLine($"[MaxterCsvImporter] CSV file not found at '{_CsvPath}'.");
            return new Dictionary<long, MaxterCsvRow>();
        }

        using StreamReader streamReader = new StreamReader(_CsvPath);
        // Row 1 of the sheet is a blank spacer row; row 2 is the real header.
        streamReader.ReadLine();

        CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            HeaderValidated = null,
        };

        using CsvReader csv = new CsvReader(streamReader, config);
        List<MaxterCsvRow> allRows = csv.GetRecords<MaxterCsvRow>().ToList();

        Dictionary<long, MaxterCsvRow> merged = new Dictionary<long, MaxterCsvRow>();
        foreach (MaxterCsvRow row in allRows)
        {
            if (row.WorkshopId is not long workshopId)
                continue;

            merged[workshopId] = merged.TryGetValue(workshopId, out MaxterCsvRow? existing)
                ? MergeRows(existing, row)
                : row;
        }

        return merged;
    }

    private static MaxterCsvRow MergeRows(MaxterCsvRow first, MaxterCsvRow second)
    {
        return new MaxterCsvRow
        {
            WorkshopId = first.WorkshopId,
            MapName = Coalesce(first.MapName, second.MapName),
            Length = Coalesce(first.Length, second.Length),
            Difficulty = Coalesce(first.Difficulty, second.Difficulty),
            Theme = Coalesce(first.Theme, second.Theme),
            DrivingStyle = Coalesce(first.DrivingStyle, second.DrivingStyle),
            MapFeatures = Coalesce(first.MapFeatures, second.MapFeatures),
            CarAbilities = Coalesce(first.CarAbilities, second.CarAbilities),
            WrTech = Coalesce(first.WrTech, second.WrTech),
            WrLength = Coalesce(first.WrLength, second.WrLength),
            WrRouteDif = Coalesce(first.WrRouteDif, second.WrRouteDif),
            WrRouteStyle = Coalesce(first.WrRouteStyle, second.WrRouteStyle),
            Readability = Coalesce(first.Readability, second.Readability),
        };
    }

    private static string? Coalesce(string? first, string? second)
    {
        return string.IsNullOrWhiteSpace(first) ? second : first;
    }
}
