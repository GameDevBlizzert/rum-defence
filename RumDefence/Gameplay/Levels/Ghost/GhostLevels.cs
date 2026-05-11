using System.Collections.Generic;
using RumDefence.Gameplay.Levels.Ghost.LevelData;

namespace RumDefence.Gameplay.Levels.Ghost;

public static class GhostLevels
{
    private static GhostTheme ghost = new GhostTheme();

    private static Theme theme = new Theme
    {
        Tiles = ghost,
    };

    public static List<Level> All = new()
    {
        Level1Data.Create(theme),
        Level2Data.Create(theme),
        Level3Data.Create(theme),
        Level4Data.Create(theme)
    };
}