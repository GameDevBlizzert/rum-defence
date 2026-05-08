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
}