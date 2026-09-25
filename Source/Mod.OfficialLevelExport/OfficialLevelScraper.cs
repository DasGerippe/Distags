using System.Collections.Generic;
using System.Linq;

namespace Mod.OfficialLevelExport
{
    public class OfficialLevelScraper
    {
        private readonly string[] _BlacklistedLevelNames =
        {
            "New Level",
            "New_new_level",
        };

        public List<ScrapedOfficialLevel> ScrapeOfficialLevels()
        {
            List<ScrapedOfficialLevel> scrapedOfficialLevels = G.Sys.LevelSets_.OfficialLevelInfosList_
                .Where(levelInfo => levelInfo.levelType_ == LevelType.Official || levelInfo.levelType_ == LevelType.Community)
                .Where(levelInfo => !G.Sys.LevelSets_.BlackListedArcadeFileNames_.Contains(levelInfo.levelName_))
                .Where(levelInfo => !_BlacklistedLevelNames.Contains(levelInfo.levelName_))
                .OrderBy(level => level.levelName_)
                .Select(levelInfo => new ScrapedOfficialLevel
                {
                    Name = levelInfo.levelName_,
                    CreatorName = GetCreatorName(levelInfo),
                    GameModes = GetGameModeNames(levelInfo),
                })
                .ToList();

            return scrapedOfficialLevels;
        }

        private string GetCreatorName(LevelInfo levelInfo)
        {
            return string.IsNullOrEmpty(levelInfo.levelCreatorName_)
                ? "Refract"
                : levelInfo.levelCreatorName_;
        }

        private string[] GetGameModeNames(LevelInfo levelInfo)
        {
            List<GameModeID> relevantGameModeIds = GameModeHelper.GetRelevantGameModeIds(levelInfo);
            string[] gameModeNames = GameModeHelper.GetGameModeNames(relevantGameModeIds);
            return gameModeNames;
        }
    }
}
