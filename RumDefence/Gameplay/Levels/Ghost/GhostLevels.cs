using System.Collections.Generic;
using RumDefence.Gameplay.Levels.Ghost.LevelData;

namespace RumDefence.Levels.Ghost;

public static class GhostLevels
{
    private static GhostTheme ghost = new GhostTheme();

    private static Theme theme = new Theme
    {
        Tiles = ghost,
        Walls = ghost
    };

    private static List<Level> _all;

    public static List<Level> All
    {
        get
        {
            if (_all == null)
            {
                _all = new List<Level>()
                {
                    Level1Data.Create(theme, true),
                    Level2Data.Create(theme),
                    Level3Data.Create(theme),
                    Level4Data.Create(theme)
                };

                string[] keys = { "ghost_1", "ghost_2", "ghost_3", "ghost_4" };
                for (int i = 0; i < _all.Count; i++)
                {
                    _all[i].SaveKey = keys[i];
                    if (SaveManager.CurrentSave.UnlockedLevelKeys.Contains(keys[i]))
                        _all[i].IsUnlocked = true;
                }
            }
            return _all;
        }
    }
}