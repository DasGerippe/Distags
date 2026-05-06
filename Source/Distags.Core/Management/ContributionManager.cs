using Distags.Core.Entities;

namespace Distags.Core.Management;

public class ContributionManager
{
    public Contribution CreateContribution(
        Level level, Tag tag, User? user, App? app, ChangeAction action, List<ContributionChange> changes)
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