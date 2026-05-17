using System.Collections.Generic;
using RumDefence.Gameplay.Levels.Grass.LevelData;

namespace RumDefence.Levels.Grass;

public static class GrassLevels
{
    private static GrassTheme grass = new GrassTheme();

    private static Theme theme = new Theme
    {
        Tiles = grass,
        Walls = grass
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

                string[] keys = { "grass_1", "grass_2", "grass_3", "grass_4" };
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