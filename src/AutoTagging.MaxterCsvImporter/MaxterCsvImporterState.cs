namespace AutoTagging.MaxterCsvImporter;

public class MaxterCsvImporterState
{
    public Dictionary<long, List<string>> AppliedTagsLog { get; set; } = [];
}
