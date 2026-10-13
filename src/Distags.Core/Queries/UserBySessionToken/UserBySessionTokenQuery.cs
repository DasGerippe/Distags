using Distags.Contracts;

namespace Distags.Core.Queries.UserBySessionToken;

public class UserBySessionTokenQuery : IQuery<UserDto?>
{
    public string SessionToken { get; set; } = string.Empty;
}
