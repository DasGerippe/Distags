using Distags.Api.Contracts.UserBans;
using Distags.Contracts;
using Riok.Mapperly.Abstractions;

namespace Distags.Api.Mapping;

[Mapper]
public static partial class UserBanMapper
{
    public static partial UserBanResponse Map(UserBanDto userBan);
}
