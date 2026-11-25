using AutoTagging.Abstractions;
using Distags.Contracts;
using System.Text.RegularExpressions;

namespace AutoTagging.Event;

public partial class LevelNameEventCalculator : ITagDeltaCalculator
{
    [GeneratedRegex(@"^\W*DAC[^A-Z].*$", RegexOptions.IgnoreCase)]
    private static partial Regex DacRegex { get; }

    public Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default)
    {
        TagDelta tagDelta = new TagDelta();
        
        if (level is not WorkshopLevelDto workshopLevel || workshopLevel.CreatedOn.Month != 12 || !DacRegex.IsMatch(level.Name))
            return Task.FromResult(tagDelta);

        string eventTagName = $"dac-{workshopLevel.CreatedOn.Year}";
        if (!level.Tags.Contains(eventTagName))
            tagDelta.AddTags.Add(eventTagName);

        return Task.FromResult(tagDelta);
    }
}
