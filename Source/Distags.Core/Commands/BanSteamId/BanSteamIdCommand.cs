namespace Distags.Core.Commands.BanSteamId;

public class BanSteamIdCommand : ICommand
{
    public long SteamId { get; set; }

    public long BannerSteamId { get; set; }

    public string? Reason { get; set; }
}