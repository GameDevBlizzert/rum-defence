using System.Collections.Generic;
using System.Linq;

namespace RumDefence;

public class TowerUnlockManager
{
    private readonly List<TowerUnlockEntry> schedule;
    private readonly InfoPopupOverlay popup;
    private readonly HashSet<TowerType> unlockedThisRun = new();

    public TowerType? PendingFreeTower { get; private set; }
    public System.Action<TowerType> OnTowerUnlocked;

    public TowerUnlockManager(int levelId, InfoPopupOverlay popup)
    {
        schedule = GrassTowerUnlockSchedule.ForLevel(levelId);
        this.popup = popup;

        foreach (var data in TowerFactory.All)
            if (SaveManager.IsTowerUnlocked(data.Type))
                unlockedThisRun.Add(data.Type);
    }

    public void CheckStartUnlock()
    {
        foreach (var entry in schedule.Where(e => e.TriggerWave == null))
            TryUnlock(entry.Tower);
    }

    public void CheckWaveUnlock(int currentWave)
    {
        foreach (var entry in schedule.Where(e => e.TriggerWave == currentWave))
            TryUnlock(entry.Tower);
    }

    public void ClearFreeTower()
    {
        PendingFreeTower = null;
    }

    private void TryUnlock(TowerType tower)
    {
        if (SaveManager.IsTowerUnlocked(tower))
            return;

        SaveManager.MarkTowerUnlocked(tower);
        unlockedThisRun.Add(tower);

        if (!SaveManager.HasClaimedFreeTowerPlacement(tower))
            PendingFreeTower = tower;

        var data = TowerFactory.All.First(t => t.Type == tower);
        popup.Show($"New tower unlocked: {data.Label}", data.Description);

        OnTowerUnlocked?.Invoke(tower);
    }

    public bool IsAvailable(TowerType tower) => unlockedThisRun.Contains(tower);
}
