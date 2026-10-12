using Distags.Contracts;
using Distags.Core.Entities;

namespace Distags.Core.Management;

public class ContributionManager
{
    public Contribution CreateContribution(
        Level level, Tag tag, User? user, App? app, TagChangeAction action, List<TagChange> changes)
    {
        return new Contribution
        {
            Level = level,
            Tag = tag,
            Action = action,
            User = user,
            App = app,
            ContributedOn = DateTimeOffset.UtcNow,
            Changes = changes,
        };
    }
}
