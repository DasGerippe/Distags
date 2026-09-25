using System.Collections.Generic;
using System.Linq;

namespace Mod.OfficialLevelExport
{
    public class GameModeHelper
    {
        private static readonly HashSet<GameModeID> RelevantGameModeIds = new HashSet<GameModeID>
        {
            GameModeID.Sprint,
            GameModeID.Challenge,
            GameModeID.Stunt,
            GameModeID.ReverseTag,
            GameModeID.MainMenu,
        };

        private static readonly Dictionary<GameModeID, string> GameModeNames = new Dictionary<GameModeID, string>
        {
            { GameModeID.None, "None" },
            { GameModeID.Sprint, SprintMode.ModeName_ },
            { GameModeID.Stunt, StuntMode.ModeName_ },
            { GameModeID.Soccer, SoccerMode.ModeName_ },
            { GameModeID.FreeRoam, FreeRoamMode.ModeName_ },
            { GameModeID.ReverseTag, ReverseTagMode.ModeName_ },
            { GameModeID.LevelEditorPlay, LevelEditorPlayMode.ModeName_ },
            { GameModeID.CoopSprint, CoopSprintMode.ModeName_ },
            { GameModeID.Challenge, ChallengeMode.ModeName_ },
            { GameModeID.Adventure, AdventureMode.ModeName_ },
            { GameModeID.SpeedAndStyle, SpeedAndStyleMode.ModeName_ },
            { GameModeID.Trackmogrify, TrackmogrifyMode.ModeName_ },
            { GameModeID.Demo, DemoMode.ModeName_ },
            { GameModeID.MainMenu, MainMenuMode.ModeName_ },
            { GameModeID.LostToEchoes, LostToEchoesMode.ModeName_ },
            { GameModeID.Nexus, NexusMode.ModeName_ },
            { GameModeID.TheOtherSide, TheOtherSideMode.ModeName_ },
            { GameModeID.Count, "Count" },
        };

        public static List<GameModeID> GetRelevantGameModeIds(LevelInfo levelInfo)
        {
            List<GameModeID> relevantGameModeIds = levelInfo.GetSupportedModes()
                .Where(RelevantGameModeIds.Contains)
                .ToList();

            if (relevantGameModeIds.Count == 0)
                relevantGameModeIds.Add(GameModeID.Sprint);

            return relevantGameModeIds;
        }

        public static string[] GetGameModeNames(IEnumerable<GameModeID> gameModeIds)
        {
            return GameModeNames
                .Where(gameModeName => gameModeIds.Contains(gameModeName.Key))
                .Select(gameModeName => gameModeName.Value)
                .ToArray();
        }
    }
}
