using System;

namespace DistagsShortcut
{
    public class UrlOpenerFactory
    {
        public IUrlOpener Create(UrlTarget urlTarget)
        {
            switch (urlTarget)
            {
                case UrlTarget.SteamOverlay:
                    return new UrlOverlayOpener();
                case UrlTarget.DefaultBrowser:
                    return new UrlDefaultBrowserOpener();
                default:
                    throw new ArgumentOutOfRangeException(nameof(urlTarget), urlTarget, null);
            }
        }
    }
}
