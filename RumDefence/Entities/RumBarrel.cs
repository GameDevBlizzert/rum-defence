using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class RumBarrel : Entity, ICollidable
{
    private readonly float _sizeinTiles;
    private readonly int _size;
    private readonly int _SpriteFrameSize;

    public HealthComponent Health { get; private set; }
    public Action<float> OnDamageTaken { get; set; }

    public RumBarrel(Vector2 position, int initialHealth = 100)
    {
        Health = new HealthComponent(initialHealth);
        Position = position;
        _sizeinTiles = 0.8f;
        Size = SizeSystem.Square(_sizeinTiles);
        _size = (int)Size.X;
        _SpriteFrameSize = 128;
        scale = Size.X / _SpriteFrameSize;
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
