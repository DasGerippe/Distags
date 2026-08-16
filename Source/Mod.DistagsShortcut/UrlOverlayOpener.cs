using Steamworks;

namespace Mod.DistagsHotkey
{
    public class UrlOverlayOpener : IUrlOpener
    {
        public void OpenUrl(string url)
        {
            SteamFriends.ActivateGameOverlayToWebPage(url);
        }
    }
}
