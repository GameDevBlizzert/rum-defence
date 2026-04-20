using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class RumBarrel : EntityWithHealth, ICollidable
{
    private readonly int _size;

    public RumBarrel(Vector2 position, int size) : base(size, size)
    {
        Position = position;
        _size = size;
        Size = new Vector2(size, size);
        Texture = RumGame.Instance.Content.Load<Texture2D>("Art/Objects/RumBarrel");
        ApplySize();
    }

    public Collider Collider => new RectangleCollider(new Rectangle(
        (int)Position.X,
        (int)Position.Y,
        _size,
        _size
    ));
}
