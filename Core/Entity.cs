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

    protected Color color = Color.White;
    protected Rectangle? sourceRectangle = null;
    protected Rectangle[] sourceRectangles = null;
    protected SpriteEffects spriteEffect = SpriteEffects.None;
    protected float layerDepth = 0f;
    protected void ApplySize()
    {
        if (Texture == null) return;

        scale = SizeSystem.ToScale(Texture, Size);
    }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Texture == null) return;

        if (sourceRectangles == null || sourceRectangles.Length == 0)
        {
            DrawLayer(spriteBatch, sourceRectangle);
        }
        else
        {
            foreach (var rect in sourceRectangles)
            {
                DrawLayer(spriteBatch, rect);
            }
        }
    }
    private void DrawLayer(SpriteBatch spriteBatch, Rectangle? source)
    {
        spriteBatch.Draw(
            Texture,
            Position,
            source,
            color,
            rotation + rotationOffset,
            origin,
            scale,
            spriteEffect,
            layerDepth
        );
    }
}