namespace Distags.Api;

internal static class Constants
{
    internal static class Authentication
    {
        internal static class SessionTokenScheme
        {
            internal const string Name = "SessionToken";
            internal const string DisplayName = "Session Token Authentication";
        }
    }

    internal static class Cache
    {
        internal const string IgnoreAuthPolicyName = "IgnoreAuth";
    }

    internal static class Roles
    {
        internal const string Contributor = "Contributor";
    }

    internal static class Claims
    {
        internal const string SteamId = "Steam ID";
        internal const string SessionToken = "Session";
    }
}
