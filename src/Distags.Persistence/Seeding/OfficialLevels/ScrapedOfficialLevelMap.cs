using CsvHelper.Configuration;
using Distags.Core.Commands.ImportOfficialLevels;

namespace Distags.Persistence.Seeding.OfficialLevels;

public sealed class ScrapedOfficialLevelMap : ClassMap<ScrapedOfficialLevel>
{
    public ScrapedOfficialLevelMap()
    {
        Map(level => level.Name);
        Map(level => level.CreatorName);
        Map(level => level.GameModes).TypeConverter<CommaListConverter>();
    }
}
