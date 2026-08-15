using CsvHelper.Configuration.Attributes;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterCsvRow
{
    [Name("Workshop ID")]
    public long? WorkshopId { get; set; }

    [Name("Map Name")]
    public string? MapName { get; set; }

    [Name("Length")]
    public string? Length { get; set; }

    [Name("Difficulty")]
    public string? Difficulty { get; set; }

    [Name("Theme")]
    public string? Theme { get; set; }

    [Name("Driving Style")]
    public string? DrivingStyle { get; set; }

    [Name("Map Features")]
    public string? MapFeatures { get; set; }

    [Name("Car Abilities")]
    public string? CarAbilities { get; set; }

    [Name("WR Tech")]
    public string? WrTech { get; set; }

    [Name("WR Length")]
    public string? WrLength { get; set; }

    [Name("WR Route Dif")]
    public string? WrRouteDif { get; set; }

    [Name("WR Route Style")]
    public string? WrRouteStyle { get; set; }

    [Name("Readability")]
    public string? Readability { get; set; }
}
