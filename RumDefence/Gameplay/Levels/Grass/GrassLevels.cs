using System.Collections.Generic;

namespace RumDefence;

public static class GrassLevels
{
    private static GrassTheme grass = new GrassTheme();

    private static Theme theme = new Theme
    {
        Tiles = grass,
        Walls = grass
    };

    public static List<Level> All = new()
    {
        Level1Data.Create(theme),
        Level2Data.Create(theme),
        Level3Data.Create(theme),
        Level4Data.Create(theme)
    };
}