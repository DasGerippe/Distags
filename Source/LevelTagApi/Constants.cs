namespace LevelTagApi;

internal static class Constants
{
    internal static class Session
    {
        internal const string AuthenticationScheme = "SessionToken";
    }

    internal static class Steam
    {
        internal const string LoginCookieScheme = "SteamLoginCookie";
    }

    internal static class Roles
    {
        internal const string Tagger = "Tagger";
        internal const string AutoTagger = "AutoTagger";
    }

    internal static class Policies
    {
        internal const string Tagging = "TaggingPolicy";
    }

    internal static class Claims
    {
        internal const string SteamId = "Steam ID";
    }
}