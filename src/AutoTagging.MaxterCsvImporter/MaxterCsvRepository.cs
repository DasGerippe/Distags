using CsvHelper;
using CsvHelper.Configuration;
using Distags.Contracts;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterCsvRepository
{
    private readonly string _CsvPath;
    private readonly ILogger<MaxterCsvRepository> _Logger;

    private bool _IsInitialized;
    private Dictionary<string, MaxterCsvRow> _OfficialRows = [];
    private Dictionary<long, MaxterCsvRow> _WorkshopRows = [];

    public MaxterCsvRepository(string csvPath, ILogger<MaxterCsvRepository> logger)
    {
        _CsvPath = csvPath;
        _Logger = logger;
    }

    public MaxterCsvRow? FindRow(LevelDto level)
    {
        if (!_IsInitialized)
            InitializeRowDictionaries();

        return level switch
        {
            OfficialLevelDto officialLevel => _OfficialRows.GetValueOrDefault(officialLevel.Name),
            WorkshopLevelDto workshopLevel => _WorkshopRows.GetValueOrDefault(workshopLevel.WorkshopId),
            _ => throw new ArgumentOutOfRangeException(nameof(level)),
        };
    }

    public IEnumerable<MaxterCsvRow> GetAllRows()
    {
        return _OfficialRows.Values.Concat(_WorkshopRows.Values);
    }

    private void InitializeRowDictionaries()
    {
        Dictionary<string, MaxterCsvRow> officialRows = [];
        Dictionary<long, MaxterCsvRow> workshopRows = [];
        try
        {
            if (!File.Exists(_CsvPath))
            {
                _Logger.LogWarning("CSV file not found at '{filePath}'.", _CsvPath);
                return;
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

            foreach (MaxterCsvRow row in allRows)
            {
                if (row.WorkshopId == null && row.MapName != null)
                {
                    officialRows[row.MapName] = officialRows.TryGetValue(row.MapName, out MaxterCsvRow? existingRow)
                        ? MergeRows(existingRow, row)
                        : row;
                }
                else if (row.WorkshopId != null)
                {
                    workshopRows[row.WorkshopId.Value] = workshopRows.TryGetValue(row.WorkshopId.Value, out MaxterCsvRow? existingRow)
                        ? MergeRows(existingRow, row)
                        : row;
                }
            }
        }
        finally
        {
            _OfficialRows = officialRows;
            _WorkshopRows = workshopRows;
            _IsInitialized = true;
        }
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
