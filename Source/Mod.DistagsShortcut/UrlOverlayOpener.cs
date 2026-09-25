using Steamworks;

namespace DistagsShortcut
{
    public class UrlOverlayOpener : IUrlOpener
    {
        public void OpenUrl(string url)
        {
            SteamFriends.ActivateGameOverlayToWebPage(url);
        }
    }
}
