namespace LevelTagApi;

internal static class Constants
{
    internal static class Authentication
    {
        internal static class SessionTokenScheme
        {
            internal const string Name = "SessionToken";
            internal const string DisplayName = "Session Token Authentication";
        }

        internal static class ApiKeyScheme
        {
            internal const string Name = "ApiKey";
            internal const string DisplayName = "API Key Authentication";
        }
    }

    internal static class Roles
    {
        internal const string Tagger = "Tagger";
        internal const string AutoTagger = "AutoTagger";
        internal const string AppManager = "AppManager";
    }

    internal static class Policies
    {
        internal const string Tagging = "TaggingPolicy";
    }

    internal static class Claims
    {
        internal const string SteamId = "Steam ID";
        internal const string SessionToken = "Session";
    }
}