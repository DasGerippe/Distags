using Distags.Core.Entities;

namespace Distags.Infrastructure.Seeding;

public class DefaultTagProvider
{
    public IEnumerable<TagGroup> GetDefaultTagTree()
    {
        List<TagGroup> defaultTagTree = CreateDefaultTagTree();

        int tagGroupPosition = 0;

        foreach (TagGroup tagGroup in defaultTagTree)
        {
            tagGroup.Position = tagGroupPosition++;

            int tagPosition = 0;
            foreach (Tag tag in tagGroup.Tags)
            {
                tag.Position = tagPosition++;
            }
        }

        return defaultTagTree;
    }

    private List<TagGroup> CreateDefaultTagTree()
    {
        return [
            new TagGroup
            {
                Name = "Game Mode",
                Tags =
                [
                    NewTag("Sprint", isAutoTag: true),
                    NewTag("Challenge", isAutoTag: true),
                    NewTag("Stunt", isAutoTag: true),
                    NewTag("Reverse Tag", isAutoTag: true),
                    NewTag("Main Menu", isAutoTag: true),
                ],
            },
            new TagGroup
            {
                Name = "Difficulty",
                Tags =
                [
                    NewTag("Casual", exclusivity: 0),
                    NewTag("Normal", exclusivity: 0),
                    NewTag("Advanced", exclusivity: 0),
                    NewTag("Expert", exclusivity: 0),
                    NewTag("Nightmare", exclusivity: 0),
                    NewTag("Brutal", exclusivity: 0),
                    NewTag("Lunatic", exclusivity: 0),
                    NewTag("Insane", exclusivity: 0),
                    NewTag("Impossible", exclusivity: 0),
                ],
            },
            new TagGroup
            {
                Name = "Length",
                Tags = [
                    NewTag("~30 Seconds", name: "30-sec", exclusivity: 0),
                    NewTag("~1 Minute", name: "1-min", exclusivity: 0),
                    NewTag("~2 Minutes", name: "2-min", exclusivity: 0),
                    NewTag("~3 Minutes", name: "3-min", exclusivity: 0),
                    NewTag("~5 Minutes", name: "5-min", exclusivity: 0),
                    NewTag("~8 Minutes", name: "8-min", exclusivity: 0),
                    NewTag("10+ Minutes", name: "15-min", exclusivity: 0),
                    NewTag("30+ Minutes", name: "45-min", exclusivity: 0),
                    NewTag("1+ Hour", name: "hours", exclusivity: 0),
                ],
            },
            new TagGroup
            {
                Name = "Abilities",
                Tags =
                [
                    NewTag("Boost Enabled", exclusivity: 0),
                    NewTag("Boost Toggling", exclusivity: 0),
                    NewTag("Boost Disabled", exclusivity: 0),
                    NewTag("Jump Enabled", exclusivity: 1),
                    NewTag("Jump Toggling", exclusivity: 1),
                    NewTag("Jump Disabled", exclusivity: 1),
                    NewTag("Grip Enabled", exclusivity: 2),
                    NewTag("Grip Toggling", exclusivity: 2),
                    NewTag("Grip Disabled", exclusivity: 2),
                    NewTag("Wings Enabled", exclusivity: 3),
                    NewTag("Wings Toggling", exclusivity: 3),
                    NewTag("Wings Disabled", exclusivity: 3),
                    NewTag("No Heat"),
                ],
            },
            new TagGroup
            {
                Name = "Gameplay",
                Tags =
                [
                    NewTag("Standard"),
                    NewTag("Flat Driving"),
                    NewTag("Obstacles"),
                    NewTag("Transfers"),
                    NewTag("Wing Flight"),
                    NewTag("Grip Flight"),
                    NewTag("Low Drag"),
                    NewTag("Climbing"),
                    NewTag("Precision"),
                    NewTag("Tech"),
                    NewTag("Jank"),
                    NewTag("Boat"),
                    NewTag("Minigame"),
                    NewTag("Puzzle"),
                    NewTag("Maze"),
                    NewTag("Low Input"),
                    NewTag("Other", name: "other-gameplay"),
                ],
            },
            new TagGroup
            {
                Name = "Gimmicks",
                Tags =
                [
                    NewTag("None", name: "no-gimmicks"),
                    NewTag("Teleports"),
                    NewTag("Multi-Path"),
                    NewTag("Hidden Path"),
                    NewTag("Laps"),
                    NewTag("Boosters"),
                    NewTag("Jump Pads"),
                    NewTag("Force Zones"),
                    NewTag("Rails"),
                    NewTag("Gravity"),
                    NewTag("Moving Floor"),
                    NewTag("Forced Camera"),
                    NewTag("Animation"),
                    NewTag("Distortion"),
                    NewTag("Required Checkpoints"),
                    NewTag("Collectibles"),
                    NewTag("Discoverables"),
                ],
            },
            new TagGroup
            {
                Name = "Readability",
                Tags =
                [
                    NewTag("Very Readable", exclusivity: 0),
                    NewTag("Mostly Readable", exclusivity: 0),
                    NewTag("Poorly Readable", exclusivity: 0),
                    NewTag("Unreadable", exclusivity: 0),
                ],
            },
            new TagGroup
            {
                Name = "Theme",
                Tags =
                [
                    NewTag("Other/Unknown", name: "other-theme"),
                    NewTag("Distance"),
                    NewTag("Halloween"),
                    NewTag("Christmas"),
                    NewTag("Plants"),
                    NewTag("Desert"),
                    NewTag("Fire/Lava", name: "fire-lava"),
                    NewTag("Water"),
                    NewTag("Snow"),
                    NewTag("Sky"),
                    NewTag("Space"),
                    NewTag("Toy Box"),
                    NewTag("Corn"),
                    NewTag("Food"),
                    NewTag("Industrial"),
                    NewTag("Broken Symmetry"),
                    NewTag("Acceleracers"),
                    NewTag("First Level"),
                    NewTag("Video Game/Movie", name: "media-reference"),
                    NewTag("Geometric"),
                    NewTag("Abstract"),
                    NewTag("Meme"),
                    NewTag("Cityscape"),
                    NewTag("Corridor"),
                    NewTag("Spooky"),
                    NewTag("Cave"),
                    NewTag("Corrupted"),
                    NewTag("Ascent/Descent", name: "vertical"),
                    NewTag("Spaghetti"),
                    NewTag("Variety"),
                    NewTag("Clutter"),
                    NewTag("Bland"),
                    NewTag("Assets"),
                    NewTag("Terrain"),
                    NewTag("Evil"),
                    NewTag("Silly"),
                    NewTag("Spectacular"),

                    NewTag("Dark"),
                    NewTag("Dim"),
                    NewTag("Bright"),
                    NewTag("Neon"),
                    NewTag("Flashy"),
                    NewTag("High Contrast", name: "contrast"),
                    NewTag("Colorful"),
                    NewTag("Monochromatic"),
                ],
            },
            new TagGroup
            {
                Name = "Level Info",
                Tags =
                [
                    NewTag("Medals"),
                    NewTag("Missing Level Bug", isAutoTag: true),
                    NewTag("Sufficiently Tagged", name: "tagged", isAutoTag: true),
                ],
            },
            new TagGroup
            {
                Name = "WR Difficulty",
                Tags =
                [
                    NewTag("Casual", name: "casual-wr", exclusivity: 0),
                    NewTag("Normal", name: "normal-wr", exclusivity: 0),
                    NewTag("Advanced", name: "advanced-wr", exclusivity: 0),
                    NewTag("Expert", name: "expert-wr", exclusivity: 0),
                    NewTag("Nightmare", name: "nightmare-wr", exclusivity: 0),
                    NewTag("Brutal", name: "brutal-wr", exclusivity: 0),
                    NewTag("Lunatic", name: "lunatic-wr", exclusivity: 0),
                    NewTag("Insane", name: "insane-wr", exclusivity: 0),
                    NewTag("Impossible", name: "impossible-wr", exclusivity: 0),
                ],
            },
            new TagGroup
            {
                Name = "WR Length",
                Tags =
                [
                    NewTag("~30 Seconds", name: "30-sec-wr", exclusivity: 0),
                    NewTag("~1 Minute", name: "1-min-wr", exclusivity: 0),
                    NewTag("~2 Minutes", name: "2-min-wr", exclusivity: 0),
                    NewTag("~3 Minutes", name: "3-min-wr", exclusivity: 0),
                    NewTag("~5 Minutes", name: "5-min-wr", exclusivity: 0),
                    NewTag("~8 Minutes", name: "8-min-wr", exclusivity: 0),
                    NewTag("10+ Minutes", name: "15-min-wr", exclusivity: 0),
                    NewTag("30+ Minutes", name: "45-min-wr", exclusivity: 0),
                    NewTag("1+ Hour", name: "hours-wr", exclusivity: 0),
                ],
            },
            new TagGroup
            {
                Name = "WR Tech",
                Tags =
                [
                    NewTag("None", name: "no-wr-tech"),
                    NewTag("Saw Boost"),
                    NewTag("Wheelie Boost"),
                    NewTag("Thruster Reset"),
                    NewTag("Bounce"),
                    NewTag("Bomb Boost"),
                    NewTag("Sliced"),
                    NewTag("MP Reset Bug"),
                    NewTag("Broken WR"),
                ],
            },
        ];
    }

    private Tag NewTag(string displayName, string? name = null, int? exclusivity = null, bool isAutoTag = false)
    {
        return new Tag
        {
            Name = name ?? displayName.ToLower().Replace(' ', '-'),
            DisplayName = displayName,
            Exclusivity = exclusivity,
            IsAutoTag = isAutoTag,
        };
    }
}