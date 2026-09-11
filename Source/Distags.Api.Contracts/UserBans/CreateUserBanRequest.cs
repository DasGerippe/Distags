namespace Distags.Api.Contracts.UserBans;

public class CreateUserBanRequest
{
    public long SteamId { get; set; }

    public string? Reason { get; set; }
}
