using UnityEngine;

namespace Mod.DistagsHotkey
{
    public class UrlDefaultBrowserOpener : IUrlOpener
    {
        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }
    }
}
