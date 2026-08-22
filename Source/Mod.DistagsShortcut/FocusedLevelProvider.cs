using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace Mod.DistagsHotkey
{
    public class FocusedLevelProvider
    {
        private readonly FieldInfo _LevelSelectMenuAbstract_LsmField;
        private readonly FieldInfo _LevelGridGrid_IsGridPushedField;
        private readonly FieldInfo _LevelGridMenu_SelectedLevelEntryField;
        private readonly FieldInfo _LevelSelectMenuLogic_SelectedEntryField;

        public FocusedLevelProvider()
        {
            _LevelSelectMenuAbstract_LsmField = AccessTools.Field(typeof(LevelSelectLeaderboardMenu), "lsm_");
            _LevelGridGrid_IsGridPushedField = AccessTools.Field(typeof(LevelGridGrid), "isGridPushed_");
            _LevelGridMenu_SelectedLevelEntryField = AccessTools.Field(typeof(LevelGridMenu), "selectedLevelEntry_");
            _LevelSelectMenuLogic_SelectedEntryField = AccessTools.Field(typeof(LevelSelectMenuLogic), "selectedEntry_");
        }

        public string GetFocusedLevelPathRelative()
        {
            string currentModeName = G.Sys.GameManager_.ModeName_;
            if (!string.IsNullOrEmpty(currentModeName))
            {
                return currentModeName != LevelEditorPlayMode.ModeName_
                    ? Resource.GetRelativeLevelPath(G.Sys.GameManager_.LevelPath_)
                    : null;
            }

            LevelSelectLeaderboardMenu levelLeaderboardMenu = Object.FindObjectOfType<LevelSelectLeaderboardMenu>();
            if (levelLeaderboardMenu != null)
                return GetFocusedLevelPathRelative(levelLeaderboardMenu);

            LevelGridMenu levelGridMenu = Object.FindObjectOfType<LevelGridMenu>();
            if (levelGridMenu != null)
                return GetFocusedLevelPathRelative(levelGridMenu);

            // Check for LevelSelectMenuLogic last, because it seems to always exist.
            LevelSelectMenuLogic levelListMenu = Object.FindObjectOfType<LevelSelectMenuLogic>();
            if (levelListMenu != null)
                return GetFocusedLevelPathRelative(levelListMenu);
            
            return null;
        }

        private string GetFocusedLevelPathRelative(LevelSelectLeaderboardMenu levelLeaderboardMenu)
        {
            LevelSelectMenuAbstract baseMenu = (LevelSelectMenuAbstract)_LevelSelectMenuAbstract_LsmField.GetValue(levelLeaderboardMenu);
            switch (baseMenu)
            {
                case LevelGridMenu levelGridMenu:
                    return GetFocusedLevelPathRelative(levelGridMenu);
                case LevelSelectMenuLogic levelListMenu:
                    return GetFocusedLevelPathRelative(levelListMenu);
                default:
                    return null;
            }
        }

        private string GetFocusedLevelPathRelative(LevelGridMenu levelGridMenu)
        {
            bool isGridPushed = (bool)_LevelGridGrid_IsGridPushedField.GetValue(levelGridMenu.levelGridGrid_);
            if (!isGridPushed)
                return null;

            LevelGridGrid.LevelEntry selectedLevel =
                (LevelGridGrid.LevelEntry)_LevelGridMenu_SelectedLevelEntryField.GetValue(levelGridMenu);
            return Resource.GetRelativeLevelPath(selectedLevel.AbsolutePath_);
        }

        private string GetFocusedLevelPathRelative(LevelSelectMenuLogic levelListMenu)
        {
            LevelSelectMenuLogic.LevelEntry selectedLevel =
                (LevelSelectMenuLogic.LevelEntry)_LevelSelectMenuLogic_SelectedEntryField.GetValue(levelListMenu);
            return selectedLevel != null ? Resource.GetRelativeLevelPath(selectedLevel.absolutePath_) : null;
        }
    }
}
