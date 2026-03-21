using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public abstract class Entity
{
    public Vector2 Position;

    protected Texture2D Texture;

    protected float rotation;
    protected float rotationOffset;

    protected Vector2 origin;

    protected float scale = 1f;

    public Vector2 Size;

    protected void ApplySize()
    {
        if (Texture == null) return;

        scale = SizeSystem.ToScale(Texture, Size);
    }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Texture == null) return;

        spriteBatch.Draw(
            Texture,
            Position,
            null,
            Color.White,
            rotation + rotationOffset,
            origin,
            scale,
            SpriteEffects.None,
            0f
        );
    }
}