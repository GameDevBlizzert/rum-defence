using System.Collections.Generic;
using RumDefence.Gameplay.Levels.Infinity.LevelData;

namespace RumDefence.Levels.Infinity;

public static class InfinityLevels
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
                    Level2Data.Create(theme, true),
                    Level3Data.Create(theme, true),
                    Level4Data.Create(theme, true),
                };

                string[] keys = { "infinity_1", "infinity_2", "infinity_3", "infinity_4" };
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
