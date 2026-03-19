using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public abstract class Screen
{
    protected ScreenManager manager;

    public Screen(ScreenManager manager)
    {
        this.manager = manager;
    }

    public virtual void Load() { }
    public virtual void Update(GameTime gameTime) { }
    public virtual void Draw(SpriteBatch spriteBatch, Matrix scale) { }
}