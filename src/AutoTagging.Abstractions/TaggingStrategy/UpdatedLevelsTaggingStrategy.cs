using Distags.Contracts;

namespace AutoTagging.Abstractions.TaggingStrategy;

public class UpdatedLevelsTaggingStrategy : CursorTaggingStrategy
{
    public UpdatedLevelsTaggingStrategy()
        : base(level => level.RecordUpdatedOn, LevelSortStrategy.RecordUpdatedAsc)
    {
    }
}
