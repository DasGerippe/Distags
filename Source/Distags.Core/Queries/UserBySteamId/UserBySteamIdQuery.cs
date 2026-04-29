using Distags.Core.Dtos;

namespace Distags.Core.Queries.UserBySteamId;

public class UserBySteamIdQuery : IQuery<UserDto?>
{
    public long SteamId { get; set; }
}