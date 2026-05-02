namespace Distags.Core.Commands.RevertContributions;

public class RevertContributionsCommand : ICommand
{
    public long UserSteamId { get; set; }

    public DateTimeOffset? RevertStartDate { get; set; }
}