using Distags.Contracts;

namespace AutoTagging.Length;

public class LevelStatsLookup
{
    private readonly Dictionary<string, LevelStats> _OfficialLevelStats;
    private readonly Dictionary<long, LevelStats> _WorkshopLevelStats;

    public LevelStatsLookup(IReadOnlyCollection<LevelStats> levelStats)
    {
         _OfficialLevelStats = levelStats
            .Where(stats => stats.LevelId < 0)
            .ToDictionary(stats => stats.LevelName);

         _WorkshopLevelStats = levelStats
             .Where(stats => stats.LevelId > 0)
             .ToDictionary(stats => stats.LevelId);
    }

    public LevelStats? GetLevelStats(LevelDto level)
    {
        return level switch
        {
            OfficialLevelDto officialLevel => _OfficialLevelStats.GetValueOrDefault(officialLevel.Name),
            WorkshopLevelDto workshopLevel => _WorkshopLevelStats.GetValueOrDefault(workshopLevel.WorkshopId),
            _ => throw new ArgumentOutOfRangeException(nameof(level)),
        };
    }
}
