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

    public static Vector2 GetMousePositionScaled()
    {
        var mouse = Mouse.GetState();

        float scaleX = (float)RumGame.Instance.GraphicsDevice.Viewport.Width / RumGame.VirtualWidth;
        float scaleY = (float)RumGame.Instance.GraphicsDevice.Viewport.Height / RumGame.VirtualHeight;

        return new Vector2(
            mouse.X / scaleX,
            mouse.Y / scaleY
        );
    }

    public void Update(GameTime gameTime)
    {
        currentScreen?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch, Matrix scale)
    {
        spriteBatch.Begin(transformMatrix: scale);

        currentScreen.Draw(spriteBatch);

        spriteBatch.End();
    }
}