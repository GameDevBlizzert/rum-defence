using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public abstract class Entity : GameObject
{
    public Vector2 Position;
    public Texture2D Texture;

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Texture != null)
            spriteBatch.Draw(Texture, Position, Color.White);
    }
}