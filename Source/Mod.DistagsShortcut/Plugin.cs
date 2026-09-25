using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Mod.DistagsShortcut.Properties;
using System;
using UnityEngine;

namespace Mod.DistagsShortcut
{
    [BepInPlugin(ModInfo.Id, ModInfo.Name, ModInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private ConfigEntry<string> _DistagsUrlConfigEntry;
        private ConfigEntry<UrlTarget> _DistagsTargetConfigEntry;
        private ConfigEntry<KeyboardShortcut> _DistagsShortcutConfigEntry;

        private readonly UrlOpenerFactory _UrlOpenerFactory = new UrlOpenerFactory();
        private DistagsOpener _DistagsOpener;

        private FocusedLevelProvider _FocusedLevelProvider;

        internal new static ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            Logger = base.Logger;

            BindConfigEntries();

            IUrlOpener urlOpener = _UrlOpenerFactory.Create(_DistagsTargetConfigEntry.Value);
            _DistagsOpener = new DistagsOpener(_DistagsUrlConfigEntry.Value, urlOpener);

            _FocusedLevelProvider = new FocusedLevelProvider();

            _DistagsUrlConfigEntry.SettingChanged += OnSettingChanged;
            _DistagsTargetConfigEntry.SettingChanged += OnSettingChanged;
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

        private void OnSettingChanged(object sender, EventArgs args)
        {
            if (!(args is SettingChangedEventArgs settingChangedArgs))
                return;

            if (settingChangedArgs.ChangedSetting.Definition == _DistagsUrlConfigEntry.Definition)
                _DistagsOpener.DistagsUrl = _DistagsUrlConfigEntry.Value;

            if (settingChangedArgs.ChangedSetting.Definition == _DistagsTargetConfigEntry.Definition)
                _DistagsOpener.UrlOpener = _UrlOpenerFactory.Create(_DistagsTargetConfigEntry.Value);
        }

        private void Update()
        {
            if (!_DistagsShortcutConfigEntry.Value.IsUp())
                return;

            string focusedLevelPathRelative = _FocusedLevelProvider.GetFocusedLevelPathRelative();
            if (string.IsNullOrEmpty(focusedLevelPathRelative))
            {
                Logger.LogWarning("Could not determine level to be opened in Distags.");
                _DistagsOpener.OpenDistags();
            }
            else
            {
                _DistagsOpener.OpenDistagsLevel(focusedLevelPathRelative);
            }
        }
    }
}
