namespace Mod.DistagsShortcut
{
    public class DistagsOpener
    {
        private string _DistagsUrl;

        public string DistagsUrl
        {
            get => _DistagsUrl;
            set => _DistagsUrl = value.TrimEnd('/');
        }

        public IUrlOpener UrlOpener { get; set; }

        public DistagsOpener(string distagsUrl, IUrlOpener urlOpener)
        {
            DistagsUrl = distagsUrl;
            UrlOpener = urlOpener;
        }

        public void OpenDistags()
        {
            OpenDistags(string.Empty);
        }

        public void OpenDistagsLevel(string levelPathRelative)
        {
            if (levelPathRelative.StartsWith(Resource.officialLevelsFolder_) || levelPathRelative.StartsWith(Resource.communityLevelsFolder_))
            {
                string levelName = G.Sys.LevelSets_.GetLevelName(levelPathRelative);
                string levelSlug = SlugHelper.CreateSlug(levelName);
                if (!string.IsNullOrEmpty(levelSlug))
                    OpenDistagsOfficialLevel(levelSlug);
                else
                    Plugin.Logger.LogWarning($"Could not get name of level: {levelPathRelative}");
            }
            else if (levelPathRelative.StartsWith(Resource.workshopFolder_))
            {
                string levelIdString = G.Sys.SteamworksManager_.UGC_.GetPublishedFileIDStringFromRelativePath(levelPathRelative);
                if (!string.IsNullOrEmpty(levelIdString) && long.TryParse(levelIdString, out long levelId))
                    OpenDistagsWorkshopLevel(levelId);
                else
                    Plugin.Logger.LogWarning($"Could not get workshop ID of level: {levelPathRelative}");
            }
            else
            {
                Plugin.Logger.LogWarning($"Level not supported by Distags: {levelPathRelative}");
            }
        }

        public void OpenDistagsOfficialLevel(string levelSlug)
        {
            string levelPath = $"{DistagsUrl}/level/official/{levelSlug}";
            OpenDistags(levelPath);
        }

        public void OpenDistagsWorkshopLevel(long levelWorkshopId)
        {
            string levelPath = $"{DistagsUrl}/level/workshop/{levelWorkshopId}";
            OpenDistags(levelPath);
        }

        private void OpenDistags(string resourcePath)
        {
            string url = $"{DistagsUrl}{resourcePath}";
            Plugin.Logger.LogDebug($"Opening '{url}'.");
            UrlOpener.OpenUrl(url);
        }
    }
}
