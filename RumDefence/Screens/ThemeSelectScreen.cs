using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class ThemeSelectScreen : Screen
{
    private SimpleButton grassButton;
    private SimpleButton stoneButton;

    private SpriteFont font;

    public ThemeSelectScreen(ScreenManager manager) : base(manager) { }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");

        grassButton = new SimpleButton(font, "Grass",
            new Vector2(800, 400), new Vector2(300, 100));

        stoneButton = new SimpleButton(font, "Stone",
            new Vector2(800, 550), new Vector2(300, 100));

        grassButton.OnClick = () =>
        {
            manager.SetScreen(new LevelSelectScreen(manager, GrassLevels.All));
        };

        stoneButton.OnClick = () =>
        {
            manager.SetScreen(new LevelSelectScreen(manager, GrassLevels.All)); // later StoneLevels
        };

        // Play theme music
        AudioManager.Instance.PlayBackgroundMusic("WhatCloudsAreMadeOf");
    }

    public override void Update(GameTime gameTime)
    {
        grassButton.Update(gameTime);
        stoneButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(new Color(30, 30, 30));

        grassButton.Draw(spriteBatch);
        stoneButton.Draw(spriteBatch);
    }
}
