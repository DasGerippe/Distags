using Distags.Api.Contracts.Levels;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper]
[UseStaticMapper(typeof(LevelMapper))]
public static partial class LevelsPageMapper
{
    public static partial LevelsPageResponse Map(LevelsPage levelsPage);
}
