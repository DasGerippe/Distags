using Distags.Contracts;

namespace Distags.Core.Entities;

public class Contribution
{
    public long Id { get; set; }

    public long LevelId { get; set; }

    public Level Level { get; set; }

    public long TagId { get; set; }

    public Tag Tag { get; set; }

    public long? UserId { get; set; }

    public User? User { get; set; }

    public long? AutoTaggerId { get; set; }

    public AutoTagger? AutoTagger { get; set; }

    public TagChangeAction Action { get; set; }

    public DateTimeOffset ContributedOn { get; set; }

    public List<TagChange> Changes { get; set; }

    public bool IsReverted { get; set; }
}
