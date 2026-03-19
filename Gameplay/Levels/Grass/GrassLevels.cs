using System.Collections.Generic;

namespace RumDefence;

public static class GrassLevels
{
    private static ITileTheme theme = new GrassTheme();

    public static List<Level> All = new()
    {
        Level1Data.Create(theme)
    };
}