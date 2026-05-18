using System.Collections.Generic;

namespace RumDefence.Gameplay.Levels.Dev;

public static class DevLevels
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
    };
}