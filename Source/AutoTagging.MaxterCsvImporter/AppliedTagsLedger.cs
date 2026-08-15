using System.Text.Json;

namespace AutoTagging.MaxterCsvImporter;

/// <summary>
/// Tracks exactly which tag names this plugin previously applied to each level, so reruns only
/// ever remove tags this plugin itself added - never a tag from another auto-tagger or a human curator.
/// </summary>
public class AppliedTagsLedger
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = true };

    private readonly string _Path;
    private readonly Lazy<Dictionary<long, List<string>>> _Entries;

    public AppliedTagsLedger(string path)
    {
        _Path = path;
        _Entries = new Lazy<Dictionary<long, List<string>>>(Load);
    }

    public IReadOnlyCollection<string> GetPreviouslyApplied(long levelId)
    {
        return _Entries.Value.TryGetValue(levelId, out List<string>? tags) ? tags : Array.Empty<string>();
    }

    public void SetCurrentlyApplied(long levelId, IReadOnlyCollection<string> tags)
    {
        if (tags.Count == 0)
            _Entries.Value.Remove(levelId);
        else
            _Entries.Value[levelId] = tags.ToList();

        Save();
    }

    private Dictionary<long, List<string>> Load()
    {
        if (!File.Exists(_Path))
            return new Dictionary<long, List<string>>();

        try
        {
            string json = File.ReadAllText(_Path);
            return JsonSerializer.Deserialize<Dictionary<long, List<string>>>(json) ?? new Dictionary<long, List<string>>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MaxterCsvImporter] Could not read applied-tags ledger at '{_Path}', starting fresh: {ex.Message}");
            return new Dictionary<long, List<string>>();
        }
    }

    private void Save()
    {
        string json = JsonSerializer.Serialize(_Entries.Value, JsonOptions);
        File.WriteAllText(_Path, json);
    }
}
