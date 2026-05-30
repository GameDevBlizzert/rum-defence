using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class RumBarrel : Entity, ICollidable
{
    private readonly int _size;

    public HealthComponent Health { get; private set; }
    public Action<float> OnDamageTaken { get; set; }

    public RumBarrel(Vector2 position, int size, int initialHealth = 100)
    {
        Health = new HealthComponent(initialHealth);
        Position = position;
        _size = size;
        Size = new Vector2(size, size);
        Texture = RumGame.Instance.Content.Load<Texture2D>("Art/Objects/RumBarrel");
    }

    public void TakeDamage(float amount)
    {
        Health.TakeDamage(amount);
        OnDamageTaken?.Invoke(amount);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        Health.DrawHealth(spriteBatch, Position, _size, _size);
    }

    public Collider Collider => new RectangleCollider(new Rectangle(
        (int)Position.X,
        (int)Position.Y,
        _size,
        _size
    ));
}
