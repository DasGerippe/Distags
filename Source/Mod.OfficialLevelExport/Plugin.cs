using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using OfficialLevelExport.Properties;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;

namespace OfficialLevelExport
{
    [BepInPlugin(ModInfo.Id, ModInfo.Name, ModInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private readonly KeyboardShortcut _ExportShortcut = new KeyboardShortcut(KeyCode.Return, KeyCode.LeftControl, KeyCode.E);
        private readonly KeyboardShortcut _OpenExportFolderShortcut = new KeyboardShortcut(KeyCode.O, KeyCode.LeftControl, KeyCode.E);

        private readonly OfficialLevelScraper _OfficialLevelScraper = new OfficialLevelScraper();
        private readonly OfficialLevelExporter _OfficialLevelExporter = new OfficialLevelExporter();

        private string _PluginFolderPath;

        internal new static ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            Logger = base.Logger;

            _PluginFolderPath = Path.GetDirectoryName(Info.Location);
        }

        [SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
        private void Update()
        {
            if (_ExportShortcut.IsDown())
                ScrapeAndExportOfficialLevels();

            if (_OpenExportFolderShortcut.IsDown())
                OpenExportFolder();
        }

        private void ScrapeAndExportOfficialLevels()
        {
            string csvFilePath = Path.Combine(_PluginFolderPath, "OfficialLevels.csv");
            List<ScrapedOfficialLevel> scrapedOfficialLevels = _OfficialLevelScraper.ScrapeOfficialLevels();
            _OfficialLevelExporter.ExportOfficialLevelsToCsv(scrapedOfficialLevels, csvFilePath);

            Logger.LogDebug($"Exported official levels to '{csvFilePath}'.");
        }

        private void OpenExportFolder()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = _PluginFolderPath,
                UseShellExecute = true,
            });
        }
    }
}
