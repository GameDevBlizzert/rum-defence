using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RumDefence;

public class ScreenManager
{
    private Screen currentScreen;

    public void SetScreen(Screen screen)
    {
        currentScreen = screen;
        currentScreen.Load();
    }

    public void Update(GameTime gameTime)
    {
        currentScreen?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch, Matrix scale)
    {
        currentScreen?.Draw(spriteBatch, scale);
    }
}