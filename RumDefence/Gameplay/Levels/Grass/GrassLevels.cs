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
            }
            return _all;
        }
    }
}