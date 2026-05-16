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
            }
            return _all;
        }
    }
}