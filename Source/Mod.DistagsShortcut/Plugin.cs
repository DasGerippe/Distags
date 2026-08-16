using BepInEx;
using BepInEx.Configuration;
using Mod.DistagsHotkey.Properties;
using UnityEngine;

namespace Mod.DistagsHotkey
{
    [BepInPlugin(ModInfo.Id, ModInfo.Name, ModInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private ConfigEntry<string> _DistagsUrlConfigEntry;
        private ConfigEntry<UrlTarget> _DistagsTargetConfigEntry;
        private ConfigEntry<KeyboardShortcut> _DistagsShortcutConfigEntry;

        private void Awake()
        {
            BindConfigEntries();
        }

        private void BindConfigEntries()
        {
            _DistagsUrlConfigEntry = Config.Bind(
                "General",
                "Distags URL",
                "https://distags.com",
                "The base URL to the Distags website.");

            _DistagsTargetConfigEntry = Config.Bind(
                "General",
                "Distags Target",
                UrlTarget.SteamOverlay,
                "Specifies where the Distags website is opened.");

            _DistagsShortcutConfigEntry = Config.Bind(
                "General",
                "Distags Shortcut",
                new KeyboardShortcut(KeyCode.F1),
                "The shortcut for opening the Distags website for the current level.");
        }
    }
}
