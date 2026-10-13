using Distags.Core.Entities;
using System.Linq.Expressions;

namespace Distags.Core.Expressions;

public static class LevelExpressions
{
    public static readonly Expression<Func<Level, string?>> CreatorNameSelector = level =>
#nullable disable
        level is OfficialLevel ? ((OfficialLevel)level).CreatorName
        : level is WorkshopLevel ? ((WorkshopLevel)level).Creator.Name
        : null;
#nullable enable

    public static readonly Expression<Func<Level, DateTimeOffset?>> UpdatedOnSelector = level =>
        level is WorkshopLevel ? ((WorkshopLevel)level).UpdatedOn : default;

    public static readonly Expression<Func<Level, DateTimeOffset?>> CreatedOnSelector = level =>
        level is WorkshopLevel ? ((WorkshopLevel)level).CreatedOn : default;
}
