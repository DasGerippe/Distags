using Distags.Contracts;
using Distags.Core.Entities;

namespace Distags.Core.Management;

public class ContributionManager
{
    public Contribution CreateContribution(
        Level level, Tag tag, User? user, AutoTagger? autoTagger, TagChangeAction action, List<TagChange> changes)
    {
        return new Contribution
        {
            Level = level,
            Tag = tag,
            Action = action,
            User = user,
            AutoTagger = autoTagger,
            ContributedOn = DateTimeOffset.UtcNow,
            Changes = changes,
        };
    }
}
