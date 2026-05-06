using Distags.Contracts;

namespace Distags.Core.Queries.UserBanBySteamId;

public class UserBanBySteamIdQuery : IQuery<UserBanDto?>
{
    public long SteamId { get; set; }
}