using System.Collections.Generic;
using GrassLevelData = RumDefence.Gameplay.Levels.Grass.LevelData;

namespace RumDefence.Levels.OneHp;

public static class OneHpLevels
{
    private static GrassTheme grass = new GrassTheme();

    private static Theme theme = new Theme
    {
        Tiles = grass,
        Walls = grass
    };

    private const int OneHpStartingLives = 1;

    private static List<Level> _all;

    public static List<Level> All
    {
        get
        {
            if (_all == null)
            {
                _all = new List<Level>()
                {
                    GrassLevelData.Level1Data.Create(theme, true,  OneHpStartingLives),
                    GrassLevelData.Level2Data.Create(theme, false, OneHpStartingLives),
                    GrassLevelData.Level3Data.Create(theme, false, OneHpStartingLives),
                    GrassLevelData.Level4Data.Create(theme, false, OneHpStartingLives),
                };

                string[] keys = { "onehp_1", "onehp_2", "onehp_3", "onehp_4" };
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
