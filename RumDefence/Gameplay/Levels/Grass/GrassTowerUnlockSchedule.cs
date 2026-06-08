using System.Collections.Generic;

namespace RumDefence;

public readonly record struct TowerUnlockEntry(TowerType Tower, int? TriggerWave)
{
    public static TowerUnlockEntry AtStart(TowerType tower) => new(tower, null);
    public static TowerUnlockEntry AtWave(TowerType tower, int wave) => new(tower, wave);
}

public static class GrassTowerUnlockSchedule
{
    private static readonly Dictionary<int, List<TowerUnlockEntry>> ScheduleByLevelId = new()
    {
        [1] = new()
        {
            TowerUnlockEntry.AtStart(TowerType.Bandit),
            TowerUnlockEntry.AtWave(TowerType.Cannon, 3),
        },
        [2] = new()
        {
            TowerUnlockEntry.AtStart(TowerType.Musket),
            TowerUnlockEntry.AtWave(TowerType.Fisher, 3),
        },
        [3] = new()
        {
            TowerUnlockEntry.AtStart(TowerType.Fire),
        },
        [4] = new(),
    };

    public static List<TowerUnlockEntry> ForLevel(int levelId)
        => ScheduleByLevelId.TryGetValue(levelId, out var entries) ? entries : new List<TowerUnlockEntry>();
}
