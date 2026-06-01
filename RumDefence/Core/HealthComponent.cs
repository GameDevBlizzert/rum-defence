using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class HealthComponent
{
    public float Current { get; private set; }

    public readonly int InitialHealth;

    public HealthComponent(int initialHealth = 100)
    {
        InitialHealth = initialHealth;
        Current = initialHealth;
    }

    public bool IsDead => Current <= 0;

    /// <param name="amount">Must be positive</param>
    public void TakeDamage(float amount)
    {
        if (amount < 0) throw new ArgumentException("Damage amount must be a positive integer.");
        Current -= amount;
    }

    /// <param name="amount">Must be positive</param>
    public void Heal(int amount)
    {
        if (amount < 0) throw new ArgumentException("Heal amount must be a positive integer.");
        Current += amount;
    }

    public void DrawHealth(SpriteBatch spriteBatch, Vector2 position, int renderedWidth, int renderedHeight)
    {
        if (IsDead || InitialHealth <= 0) return;

        var healthPercentage = MathHelper.Clamp(Current / InitialHealth, 0f, 1f);

        const int barHeight = 2;
        const int barYOffset = 5;

        var barX = (int)position.X;
        var barY = (int)(position.Y + renderedHeight + barYOffset);

        spriteBatch.Draw(Primitives.Pixel, new Rectangle(barX, barY, renderedWidth, barHeight), Color.Red);
        spriteBatch.Draw(Primitives.Pixel, new Rectangle(barX, barY, (int)(renderedWidth * healthPercentage), barHeight), Color.YellowGreen);
    }
}
