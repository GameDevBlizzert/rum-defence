using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class RumBarrel : EntityWithHealth, ICollidable
{
    private readonly int _size;

    public Action<int> OnDamageTaken { get; set; }

    public RumBarrel(Vector2 position, int size, int initialHealth = 100) : base(size, size, initialHealth)
    {
        Position = position;
        _size = size;
        Size = new Vector2(size, size);
        Texture = RumGame.Instance.Content.Load<Texture2D>("Art/Objects/RumBarrel");
        ApplySize();
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        OnDamageTaken?.Invoke(amount);
    }

    public Collider Collider => new RectangleCollider(new Rectangle(
        (int)Position.X,
        (int)Position.Y,
        _size,
        _size
    ));
}
