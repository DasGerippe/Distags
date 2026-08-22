using UnityEngine;

namespace Mod.DistagsShortcut
{
    public class UrlDefaultBrowserOpener : IUrlOpener
    {
        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }
    }
}
