using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence;

public class Enemy : Entity
{
    public float Speed { get; set; } = 50f;
    public int Health { get; protected set; } = 100;

    protected List<IModifier> modifiers = new();

    public Enemy(Vector2 start, Texture2D texture)
    {
        Position = start;
        Texture = texture;
    }

    public void AddModifier(IModifier mod)
    {
        mod.Apply(this);
        modifiers.Add(mod);
    }

    public override void Update(GameTime gameTime)
    {
        // later pathfinding
    }

    public virtual void TakeDamage(int amount)
    {
        Health -= amount;
    }

    public bool IsDead => Health <= 0;
}