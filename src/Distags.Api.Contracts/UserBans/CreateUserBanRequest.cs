namespace Distags.Api.Contracts.UserBans;

public class CreateUserBanRequest
{
    public long SteamId { get; init; }

    public string? Reason { get; init; }
}
