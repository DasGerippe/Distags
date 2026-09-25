using Distags.Core.Entities;

namespace Distags.Persistence.Seeding;

public class DefaultTagProvider
{
    private readonly Dictionary<string, DisplayGroup> _DisplayGroups = [];
    private readonly Dictionary<string, LogicGroup> _LogicGroups = [];
    private readonly List<Tag> _Tags = [];

    public DefaultTagProvider()
    {
        CreateDisplayGroup("Game Mode");
        CreateDisplayGroup("Difficulty");
        CreateDisplayGroup("Length");
        CreateDisplayGroup("Abilities");
        CreateDisplayGroup("Gameplay");
        CreateDisplayGroup("Gimmicks");
        CreateDisplayGroup("Readability");
        CreateDisplayGroup("Theme");
        CreateDisplayGroup("Level Info");
        CreateDisplayGroup("WR Difficulty");
        CreateDisplayGroup("WR Length");
        CreateDisplayGroup("WR Tech");

        CreateLogicGroup("difficulty", allExclusive: true);
        CreateLogicGroup("length", allExclusive: true);
        CreateLogicGroup("boost", allExclusive: true);
        CreateLogicGroup("jump", allExclusive: true);
        CreateLogicGroup("grip", allExclusive: true);
        CreateLogicGroup("wings", allExclusive: true);
        CreateLogicGroup("cooldown", allExclusive: true);
        CreateLogicGroup("gameplay");
        CreateLogicGroup("gimmicks");
        CreateLogicGroup("readability", allExclusive: true);
        CreateLogicGroup("theme");
        CreateLogicGroup("level-type", allExclusive: true);
        CreateLogicGroup("wr-difficulty", allExclusive: true);
        CreateLogicGroup("wr-length", allExclusive: true);
        CreateLogicGroup("wr-tech");

        CreateTag("game-mode", null, "Sprint", isAutoTag: true);
        CreateTag("game-mode", null, "Challenge", isAutoTag: true);
        CreateTag("game-mode", null, "Stunt", isAutoTag: true);
        CreateTag("game-mode", null, "Reverse Tag", isAutoTag: true);
        CreateTag("game-mode", null, "Main Menu", isAutoTag: true);

        CreateTag("difficulty", "difficulty", "Unspecified", name: "difficulty-unspecified", isExclusive: true, isFallback: true);
        CreateTag("difficulty", "difficulty", "Casual", isExclusive: true);
        CreateTag("difficulty", "difficulty", "Normal", isExclusive: true);
        CreateTag("difficulty", "difficulty", "Advanced", isExclusive: true);
        CreateTag("difficulty", "difficulty", "Expert", isExclusive: true);
        CreateTag("difficulty", "difficulty", "Nightmare", isExclusive: true);
        CreateTag("difficulty", "difficulty", "Brutal", isExclusive: true);
        CreateTag("difficulty", "difficulty", "Lunatic", isExclusive: true);
        CreateTag("difficulty", "difficulty", "Insane", isExclusive: true);
        CreateTag("difficulty", "difficulty", "Impossible", isExclusive: true);

        CreateTag("length", "length", "Unspecified", name: "length-unspecified", isExclusive: true, isFallback: true);
        CreateTag("length", "length", "~30 Seconds", name: "30-sec", isExclusive: true);
        CreateTag("length", "length", "~1 Minute", name: "1-min", isExclusive: true);
        CreateTag("length", "length", "~2 Minutes", name: "2-min", isExclusive: true);
        CreateTag("length", "length", "~3 Minutes", name: "3-min", isExclusive: true);
        CreateTag("length", "length", "~5 Minutes", name: "5-min", isExclusive: true);
        CreateTag("length", "length", "~8 Minutes", name: "8-min", isExclusive: true);
        CreateTag("length", "length", "10+ Minutes", name: "15-min", isExclusive: true);
        CreateTag("length", "length", "30+ Minutes", name: "45-min", isExclusive: true);
        CreateTag("length", "length", "1+ Hour", name: "hours", isExclusive: true);

        CreateTag("abilities", "boost", "Boost Unspecified", isExclusive: true, isFallback: true);
        CreateTag("abilities", "boost", "Boost Enabled", isExclusive: true);
        CreateTag("abilities", "boost", "Boost Toggling", isExclusive: true);
        CreateTag("abilities", "boost", "Boost Disabled", isExclusive: true);
        CreateTag("abilities", "jump", "Jump Unspecified", isExclusive: true, isFallback: true);
        CreateTag("abilities", "jump", "Jump Enabled", isExclusive: true);
        CreateTag("abilities", "jump", "Jump Toggling", isExclusive: true);
        CreateTag("abilities", "jump", "Jump Disabled", isExclusive: true);
        CreateTag("abilities", "grip", "Grip Unspecified", isExclusive: true, isFallback: true);
        CreateTag("abilities", "grip", "Grip Enabled", isExclusive: true);
        CreateTag("abilities", "grip", "Grip Toggling", isExclusive: true);
        CreateTag("abilities", "grip", "Grip Disabled", isExclusive: true);
        CreateTag("abilities", "wings", "Wings Unspecified", isExclusive: true, isFallback: true);
        CreateTag("abilities", "wings", "Wings Enabled", isExclusive: true);
        CreateTag("abilities", "wings", "Wings Toggling", isExclusive: true);
        CreateTag("abilities", "wings", "Wings Disabled", isExclusive: true);
        CreateTag("abilities", "cooldown", "Cooldown Unspecified", isExclusive: true, isFallback: true);
        CreateTag("abilities", "cooldown", "Cooldown Enabled", isExclusive: true);
        CreateTag("abilities", "cooldown", "Cooldown Toggling", isExclusive: true);
        CreateTag("abilities", "cooldown", "Cooldown Disabled", isExclusive: true);

        CreateTag("gameplay", "gameplay", "Unspecified", name: "gameplay-unspecified", isExclusive: true, isFallback: true);
        CreateTag("gameplay", "gameplay", "Standard");
        CreateTag("gameplay", "gameplay", "Flat Driving");
        CreateTag("gameplay", "gameplay", "Obstacles");
        CreateTag("gameplay", "gameplay", "Transfers");
        CreateTag("gameplay", "gameplay", "Wing Flight");
        CreateTag("gameplay", "gameplay", "Grip Flight");
        CreateTag("gameplay", "gameplay", "Low Drag");
        CreateTag("gameplay", "gameplay", "Climbing");
        CreateTag("gameplay", "gameplay", "Precision");
        CreateTag("gameplay", "gameplay", "Tech");
        CreateTag("gameplay", "gameplay", "Jank");
        CreateTag("gameplay", "gameplay", "Boat");
        CreateTag("gameplay", "gameplay", "Minigame");
        CreateTag("gameplay", "gameplay", "Puzzle");
        CreateTag("gameplay", "gameplay", "Maze");
        CreateTag("gameplay", "gameplay", "Low Input");
        CreateTag("gameplay", "gameplay", "Other", name: "gameplay-other");

        CreateTag("gimmicks", "gimmicks", "Unspecified", name: "gimmicks-unspecified", isExclusive: true, isFallback: true);
        CreateTag("gimmicks", "gimmicks", "None", name: "gimmicks-none", isExclusive: true);
        CreateTag("gimmicks", "gimmicks", "Teleports");
        CreateTag("gimmicks", "gimmicks", "Multi-Path");
        CreateTag("gimmicks", "gimmicks", "Hidden Path");
        CreateTag("gimmicks", "gimmicks", "Laps");
        CreateTag("gimmicks", "gimmicks", "Boosters");
        CreateTag("gimmicks", "gimmicks", "Jump Pads");
        CreateTag("gimmicks", "gimmicks", "Force Zones");
        CreateTag("gimmicks", "gimmicks", "Rails");
        CreateTag("gimmicks", "gimmicks", "Gravity");
        CreateTag("gimmicks", "gimmicks", "Moving Floor");
        CreateTag("gimmicks", "gimmicks", "Forced Camera");
        CreateTag("gimmicks", "gimmicks", "Animation");
        CreateTag("gimmicks", "gimmicks", "Distortion");
        CreateTag("gimmicks", "gimmicks", "Required Checkpoints");
        CreateTag("gimmicks", "gimmicks", "Collectibles");
        CreateTag("gimmicks", "gimmicks", "Discoverables");

        CreateTag("readability", "readability", "Unspecified", name: "readability-unspecified", isExclusive: true, isFallback: true);
        CreateTag("readability", "readability", "Very Readable", isExclusive: true);
        CreateTag("readability", "readability", "Mostly Readable", isExclusive: true);
        CreateTag("readability", "readability", "Poorly Readable", isExclusive: true);
        CreateTag("readability", "readability", "Unreadable", isExclusive: true);

        CreateTag("theme", "theme", "Unspecified", name: "theme-unspecified", isExclusive: true, isFallback: true);
        CreateTag("theme", "theme", "Other", name: "theme-other");
        CreateTag("theme", "theme", "Distance");
        CreateTag("theme", "theme", "Halloween");
        CreateTag("theme", "theme", "Christmas");
        CreateTag("theme", "theme", "Plants");
        CreateTag("theme", "theme", "Desert");
        CreateTag("theme", "theme", "Fire");
        CreateTag("theme", "theme", "Lava");
        CreateTag("theme", "theme", "Water");
        CreateTag("theme", "theme", "Snow");
        CreateTag("theme", "theme", "Sky");
        CreateTag("theme", "theme", "Space");
        CreateTag("theme", "theme", "Toy Box");
        CreateTag("theme", "theme", "Corn");
        CreateTag("theme", "theme", "Food");
        CreateTag("theme", "theme", "Industrial");
        CreateTag("theme", "theme", "Broken Symmetry");
        CreateTag("theme", "theme", "Acceleracers");
        CreateTag("theme", "theme", "First Level");
        CreateTag("theme", "theme", "Video Game/Movie", name: "media-reference");
        CreateTag("theme", "theme", "Geometric");
        CreateTag("theme", "theme", "Abstract");
        CreateTag("theme", "theme", "Meme");
        CreateTag("theme", "theme", "Cityscape");
        CreateTag("theme", "theme", "Corridor");
        CreateTag("theme", "theme", "Spooky");
        CreateTag("theme", "theme", "Cave");
        CreateTag("theme", "theme", "Corrupted");
        CreateTag("theme", "theme", "Vertical");
        CreateTag("theme", "theme", "Spaghetti");
        CreateTag("theme", "theme", "Variety");
        CreateTag("theme", "theme", "Clutter");
        CreateTag("theme", "theme", "Bland");
        CreateTag("theme", "theme", "Assets");
        CreateTag("theme", "theme", "Terrain");
        CreateTag("theme", "theme", "Evil");
        CreateTag("theme", "theme", "Silly");
        CreateTag("theme", "theme", "Spectacular");
        CreateTag("theme", "theme", "Dark");
        CreateTag("theme", "theme", "Dim");
        CreateTag("theme", "theme", "Bright");
        CreateTag("theme", "theme", "Neon");
        CreateTag("theme", "theme", "Flashy");
        CreateTag("theme", "theme", "High Contrast", name: "contrast");
        CreateTag("theme", "theme", "Colorful");
        CreateTag("theme", "theme", "Monochromatic");

        CreateTag("level-info", "level-type", "Official Level", name: "official", isAutoTag: true, isExclusive: true);
        CreateTag("level-info", "level-type", "Workshop Level", name: "workshop", isAutoTag: true, isExclusive: true);
        CreateTag("level-info", null, "Medals");
        CreateTag("level-info", null, "Missing Level Bug", name: "missing-level", isAutoTag: true);

        CreateTag("wr-difficulty", "wr-difficulty", "Unspecified", name: "wr-difficulty-unspecified", isExclusive: true, isFallback: true);
        CreateTag("wr-difficulty", "wr-difficulty", "Casual", name: "wr-casual", isExclusive: true);
        CreateTag("wr-difficulty", "wr-difficulty", "Normal", name: "wr-normal", isExclusive: true);
        CreateTag("wr-difficulty", "wr-difficulty", "Advanced", name: "wr-advanced", isExclusive: true);
        CreateTag("wr-difficulty", "wr-difficulty", "Expert", name: "wr-expert", isExclusive: true);
        CreateTag("wr-difficulty", "wr-difficulty", "Nightmare", name: "wr-nightmare", isExclusive: true);
        CreateTag("wr-difficulty", "wr-difficulty", "Brutal", name: "wr-brutal", isExclusive: true);
        CreateTag("wr-difficulty", "wr-difficulty", "Lunatic", name: "wr-lunatic", isExclusive: true);
        CreateTag("wr-difficulty", "wr-difficulty", "Insane", name: "wr-insane", isExclusive: true);
        CreateTag("wr-difficulty", "wr-difficulty", "Impossible", name: "wr-impossible", isExclusive: true);

        CreateTag("wr-length", "wr-length", "Unspecified", name: "wr-length-unspecified", isExclusive: true, isFallback: true);
        CreateTag("wr-length", "wr-length", "~30 Seconds", name: "wr-30-sec", isExclusive: true);
        CreateTag("wr-length", "wr-length", "~1 Minute", name: "wr-1-min", isExclusive: true);
        CreateTag("wr-length", "wr-length", "~2 Minutes", name: "wr-2-min", isExclusive: true);
        CreateTag("wr-length", "wr-length", "~3 Minutes", name: "wr-3-min", isExclusive: true);
        CreateTag("wr-length", "wr-length", "~5 Minutes", name: "wr-5-min", isExclusive: true);
        CreateTag("wr-length", "wr-length", "~8 Minutes", name: "wr-8-min", isExclusive: true);
        CreateTag("wr-length", "wr-length", "10+ Minutes", name: "wr-15-min", isExclusive: true);
        CreateTag("wr-length", "wr-length", "30+ Minutes", name: "wr-45-min", isExclusive: true);
        CreateTag("wr-length", "wr-length", "1+ Hour", name: "wr-hours", isExclusive: true);

        CreateTag("wr-tech", "wr-tech", "Unspecified", name: "wr-tech-unspecified", isExclusive: true, isFallback: true);
        CreateTag("wr-tech", "wr-tech", "None", name: "wr-tech-none", isExclusive: true);
        CreateTag("wr-tech", "wr-tech", "Saw Boost");
        CreateTag("wr-tech", "wr-tech", "Wheelie Boost");
        CreateTag("wr-tech", "wr-tech", "Thruster Reset");
        CreateTag("wr-tech", "wr-tech", "Bounce");
        CreateTag("wr-tech", "wr-tech", "Bomb Boost");
        CreateTag("wr-tech", "wr-tech", "Sliced");
        CreateTag("wr-tech", "wr-tech", "MP Reset Bug");
        CreateTag("wr-tech", "wr-tech", "Broken WR");

        InitializeTagPositions();
    }

    private void CreateDisplayGroup(string displayName, string? name = null)
    {
        DisplayGroup displayGroup = new DisplayGroup
        {
            Name = name ?? displayName.ToLower().Replace(' ', '-'),
            DisplayName = displayName,
        };

        _DisplayGroups[displayGroup.Name] = displayGroup;
        displayGroup.Position = _DisplayGroups.Count;
    }

    private void CreateLogicGroup(string name, bool allExclusive = false)
    {
        LogicGroup logicGroup = new LogicGroup
        {
            Name = name,
            AllExclusive = allExclusive,
        };

        _LogicGroups[logicGroup.Name] = logicGroup;
    }

    private void CreateTag(
        string displayGroupName,
        string? logicGroupName,
        string displayName,
        string? name = null,
        bool isExclusive = false,
        bool isFallback = false,
        bool isAutoTag = false)
    {
        Tag tag = new Tag
        {
            DisplayGroup = _DisplayGroups[displayGroupName],
            LogicGroup = string.IsNullOrEmpty(logicGroupName) ? null : _LogicGroups[logicGroupName],
            Name = name ?? displayName.ToLower().Replace(' ', '-'),
            DisplayName = displayName,
            IsExclusive = isExclusive,
            IsFallback = isFallback,
            IsAutoTag = isAutoTag,
        };

        _Tags.Add(tag);
    }

    private void InitializeTagPositions()
    {
        DisplayGroup? lastDisplayGroup = null;
        int position = 1;

        foreach (Tag tag in _Tags)
        {
            if (tag.DisplayGroup != lastDisplayGroup)
                position = 1;
            else
                position++;

            tag.Position = position;
            lastDisplayGroup = tag.DisplayGroup;
        }
    }

    public IEnumerable<DisplayGroup> GetDefaultDisplayGroups()
    {
        return _DisplayGroups.Values;
    }

    public IEnumerable<LogicGroup> GetDefaultLogicGroups()
    {
        return _LogicGroups.Values;
    }

    public IEnumerable<Tag> GetDefaultTags()
    {
        return _Tags;
    }
}
