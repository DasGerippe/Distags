using Steamworks;

namespace Mod.DistagsShortcut
{
    public class UrlOverlayOpener : IUrlOpener
    {
        public void OpenUrl(string url)
        {
            SteamFriends.ActivateGameOverlayToWebPage(url);
        }
    }
}
