using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public abstract class Screen
{
    protected ScreenManager manager;
    private bool hasBeenLoaded;

    public Screen(ScreenManager manager)
    {
        this.manager = manager;
        hasBeenLoaded = false;
    }

    public virtual void Load() { }
    public virtual void Update(GameTime gameTime) { }
    public abstract void Draw(SpriteBatch spriteBatch);

    public bool HasBeenLoaded => hasBeenLoaded;

    public void MarkAsLoaded()
    {
        hasBeenLoaded = true;
    }
}
