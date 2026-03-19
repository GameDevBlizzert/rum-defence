using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RumDefence;

public class LevelSelectScreen : Screen
{
    SpriteFont font;

    public LevelSelectScreen(ScreenManager manager) : base(manager) { }

    public override void Load()
    {
        font = RumGame.Instance.Content.Load<SpriteFont>("Font");
    }

    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.D1))
        {
            manager.SetScreen(new GameScreen(manager, GrassLevels.All[0]));
        }
    }

    public override void Draw(SpriteBatch spriteBatch, Matrix scale)
    {
        spriteBatch.Begin();

        spriteBatch.DrawString(font, "Select Level", new Vector2(300, 200), Color.White);
        spriteBatch.DrawString(font, "Press 1 for Level 1", new Vector2(300, 300), Color.White);

        spriteBatch.End();
    }
}