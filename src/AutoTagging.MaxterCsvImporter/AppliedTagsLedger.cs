using Distags.Contracts;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AutoTagging.MaxterCsvImporter;

/// <summary>
/// Tracks exactly which tag names this plugin previously applied to each level, so reruns only
/// ever remove tags this plugin itself added - never a tag from another auto-tagger or a human curator.
/// </summary>
public class AppliedTagsLedger
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = true };

    private readonly string _FilePath;
    private readonly ILogger<AppliedTagsLedger> _Logger;

    private readonly Lazy<Dictionary<long, List<string>>> _Entries;

    public AppliedTagsLedger(string filePath, ILogger<AppliedTagsLedger> logger)
    {
        _FilePath = filePath;
        _Logger = logger;
        _Entries = new Lazy<Dictionary<long, List<string>>>(Load);
    }

    public IReadOnlyCollection<string> GetPreviouslyApplied(LevelDto level)
    {
        return _Entries.Value.TryGetValue(level.Id, out List<string>? tags) ? tags : [];
    }

    public void SetCurrentlyApplied(LevelDto level, IReadOnlyCollection<string> tags)
    {
        if (tags.Count == 0)
            _Entries.Value.Remove(level.Id);
        else
            _Entries.Value[level.Id] = tags.ToList();

        Save();
    }

    private Dictionary<long, List<string>> Load()
    {
        if (!File.Exists(_FilePath))
            return new Dictionary<long, List<string>>();

        try
        {
            string json = File.ReadAllText(_FilePath);
            return JsonSerializer.Deserialize<Dictionary<long, List<string>>>(json) ?? new Dictionary<long, List<string>>();
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "Could not read applied-tags ledger at '{filePath}'. Starting fresh.", _FilePath);
            return new Dictionary<long, List<string>>();
        }
    }

    private void Save()
    {
        string json = JsonSerializer.Serialize(_Entries.Value, JsonOptions);
        File.WriteAllText(_FilePath, json);
    }
}
