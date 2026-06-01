using System;
using Microsoft.Xna.Framework;

namespace RumDefence;

public class Wall
{
    public const int BaseMaxHealth = 20;
    public const int MaxUpgradeLevel = 3;

    private static readonly int[] MaxHealthByLevel = { 20, 35, 55, 80 };
    private static readonly int[] UpgradeCostByLevel = { 20, 35, 55 };

    public Point GridPos;
    public bool IsDiagonal;

    public int UpgradeLevel { get; private set; } = 0;

    public int MaxHealth => MaxHealthByLevel[UpgradeLevel];
    public int NextMaxHealth => CanUpgrade ? MaxHealthByLevel[UpgradeLevel + 1] : MaxHealth;

    public int Health { get; private set; }

    public bool IsDamaged => Health < MaxHealth;
    public bool IsDestroyed => Health <= 0;
    public bool CanUpgrade => UpgradeLevel < MaxUpgradeLevel;

    public Wall(Point gridPos, bool isDiagonal = false)
    {
        GridPos = gridPos;
        IsDiagonal = isDiagonal;
        Health = MaxHealth;
    }

    public void TakeDamage(int amount)
    {
        Health = Math.Max(0, Health - amount);
    }

    public void Repair(int amount)
    {
        if (amount <= 0) return;
        Health = Math.Min(MaxHealth, Health + amount);
    }

    public void RepairToFull()
    {
        Health = MaxHealth;
    }

    public int GetRepairCostToFull()
    {
        int missing = MaxHealth - Health;
        if (missing <= 0) return 0;

        float costPerHp = BuildManager.WallCost / (float)BaseMaxHealth;
        return (int)Math.Ceiling(missing * costPerHp);
    }

    public int GetUpgradeCost()
    {
        if (!CanUpgrade) return 0;
        return UpgradeCostByLevel[UpgradeLevel];
    }

    public void ApplyUpgrade()
    {
        if (!CanUpgrade) return;
        UpgradeLevel++;
        RepairToFull();
    }
}
