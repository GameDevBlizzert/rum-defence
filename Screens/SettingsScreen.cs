using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RumDefence;

public class SettingsScreen : Screen
{
    private Screen previous;

    public SettingsScreen(ScreenManager manager, Screen previous) : base(manager)
    {
        this.previous = previous;
    }

    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            manager.SetScreen(previous);
        }
    }

    public override void Draw(SpriteBatch spriteBatch, Matrix scale)
    {
        spriteBatch.Begin(transformMatrix: scale);

        spriteBatch.End();
    }
}