using System.Collections.Generic;

namespace RumDefence;

public class Level
{
    public int Id { get; }
    public int[,] Map { get; }
    public ITileTheme Theme { get; }
    public List<Wave> Waves { get; }
    public bool IsUnlocked { get; set; }

    public Level(
        int id,
        int[,] map,
        ITileTheme theme,
        List<Wave> waves,
        bool unlocked = false)
    {
        Id = id;
        Map = map;
        Theme = theme;
        Waves = waves;
        IsUnlocked = unlocked;
    }
}