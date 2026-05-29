using System;
using Microsoft.Xna.Framework;

namespace RumDefence;

public class Wall
{
    public const int MaxHealth = 20;

    public Point GridPos;
    public bool IsDiagonal;

    public int Health { get; private set; } = MaxHealth;
    public bool IsDamaged => Health < MaxHealth;
    public bool IsDestroyed => Health <= 0;

    public Wall(Point gridPos, bool isDiagonal = false)
    {
        GridPos = gridPos;
    }

    public void TakeDamage(int amount)
    {
        Health = Math.Max(0, Health - amount);
    }

    /// <summary>
    /// Repair the wall by a fixed amount of health.
    /// </summary>
    public void Repair(int amount)
    {
        if (amount <= 0) return;
        Health = Math.Min(MaxHealth, Health + amount);
    }

    /// <summary>
    /// Repair the wall back to full health.
    /// </summary>
    public void RepairToFull()
    {
        Health = MaxHealth;
    }

    /// <summary>
    /// Get the coin cost to repair this wall back to full health.
    /// Cost scales linearly with missing health relative to `BuildManager.WallCost`.
    /// </summary>
    public int GetRepairCostToFull()
    {
        int missing = MaxHealth - Health;
        if (missing <= 0) return 0;

        // Linearly scale: fully destroyed costs the same as building a new wall.
        float costPerHp = BuildManager.WallCost / (float)MaxHealth;
        return (int)Math.Ceiling(missing * costPerHp);
    }
}
