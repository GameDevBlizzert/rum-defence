using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public abstract class EntityWithHealth : Entity
{

    /// <summary>
    /// The health of the entity. When it reaches 0 or below, the entity is considered dead.
    /// </summary>
    public int Health
    {
        get; protected set;
    }

    /// <summary>
    /// The initial health of the entity when it is created.
    /// </summary>
    public readonly int InitialHealth;

    private readonly Texture2D whitePixelTexture2D;

    /// <summary>
    /// The width of the entity as it is rendered on the screen
    /// </summary>
    private readonly int entityRenderedWidth;

    /// <summary>
    /// The height of the entity as it is rendered on the screen.
    /// </summary>
    private readonly int entityRenderedHeight;

    protected EntityWithHealth(int renderedWidth, int renderedHeight, int initialHealth = 100)
    {
        entityRenderedWidth = renderedWidth;
        entityRenderedHeight = renderedHeight;

        InitialHealth = initialHealth;
        Health = initialHealth;

        whitePixelTexture2D = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
        whitePixelTexture2D.SetData(new[] { Color.White });
    }

    protected EntityWithHealth(int initialHealth = 100)
    {

    }

    /// <summary>
    /// Returns whether the entity is dead, which is true if Health is 0 or below.
    /// </summary>
    public bool IsDead => Health <= 0;

    /// <summary>
    /// Reduces the entity's health by the specified amount.
    /// </summary>
    /// <param name="amount">The amount of damage the entity is taking, must be a positive integer</param>
    /// <exception cref="ArgumentException">Thrown if the damage amount is negative</exception>
    public void TakeDamage(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Damage amount must be a positive integer.");
        }

        Health -= amount;
    }

    /// <summary>
    /// Increases the entity's health by the specified amount.
    /// </summary>
    /// <param name="amount">The amount of health the entity is receiving, must be a positive integer</param>
    /// <exception cref="ArgumentException">Thrown if the heal amount is negative</exception>
    public void Heal(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Heal amount must be a positive integer.");
        }
        Health += amount;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        if (IsDead || InitialHealth <= 0) return;

        var healthPercentage = MathHelper.Clamp((float)Health / InitialHealth, 0f, 1f);

        const int barHeight = 2;
        const int barYOffset = 5;

        var barX = (int)Position.X;
        var barY = (int)(Position.Y + entityRenderedHeight + barYOffset);
        var barWidth = entityRenderedWidth;
        var healthBarWidth = (int)(barWidth * healthPercentage);

        var healthRectangle = new Rectangle(0, 0, healthBarWidth, barHeight);
        var backgroundRectangle = new Rectangle(0, 0, barWidth, barHeight);

        var healthPosition = new Vector2(barX, barY);
        var backgroundPosition = new Vector2(barX, barY);

        spriteBatch.Draw(
            whitePixelTexture2D,
            backgroundPosition,
            backgroundRectangle,
            Color.Red,
            rotation + rotationOffset,
            origin,
            scale,
            spriteEffect,
            layerDepth
        );

        spriteBatch.Draw(
            whitePixelTexture2D,
            healthPosition,
            healthRectangle,
            Color.YellowGreen,
            rotation + rotationOffset,
            origin,
            scale,
            spriteEffect,
            layerDepth
        );

    }
}
