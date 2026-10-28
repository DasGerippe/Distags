using Distags.Contracts;

namespace AutoTagging.Abstractions.TaggingStrategy;

public class NewLevelsTaggingStrategy : CursorTaggingStrategy
{
    public NewLevelsTaggingStrategy()
        : base(level => level.RecordCreatedOn, LevelSortStrategy.RecordCreatedAsc)
    {
    }
}
