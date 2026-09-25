using UnityEngine;

namespace DistagsShortcut
{
    public class UrlDefaultBrowserOpener : IUrlOpener
    {
        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }
    }
}
