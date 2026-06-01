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
                    Level4Data.Create(theme),
                    Level5Data.Create(theme),
                    Level6Data.Create(theme),
                    Level7Data.Create(theme),
                    Level8Data.Create(theme),
                };

                string[] keys = { "grass_1", "grass_2", "grass_3", "grass_4", "grass_5" , "grass_6", "grass_7", "grass_8" };
                for (int i = 0; i < _all.Count; i++)
                {
                    _all[i].SaveKey = keys[i];
                }

                SaveManager.ApplySaveDataToLevels(_all);
            }
            return _all;
        }
    }
}