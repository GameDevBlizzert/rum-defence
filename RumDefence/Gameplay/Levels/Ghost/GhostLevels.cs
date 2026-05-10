using System.Collections.Generic;

namespace RumDefence.Levels.Ghost;

public static class GhostLevels
{
    private static GhostTheme ghost = new GhostTheme();
    private static GrassTheme grass = new GrassTheme();

    private static Theme theme = new Theme
    {
        Tiles = ghost,
        Walls = grass 
    };

    public static List<Level> All = new()
    {
        Level1Data.Create(theme, true),
        Level2Data.Create(theme),
        Level3Data.Create(theme),
        Level4Data.Create(theme)
    };
}