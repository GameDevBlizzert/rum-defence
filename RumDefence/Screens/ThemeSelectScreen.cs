using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.Gameplay.Levels.Dev;
using RumDefence.Gameplay.Levels.Ghost;
using RumDefence.Levels.Grass;

namespace RumDefence;

public class ThemeSelectScreen : Screen
{
    private SimpleButton grassButton;
    private SimpleButton stoneButton;
    private SimpleButton devButton;

    private Texture2D buttonTexture;

    public ThemeSelectScreen(ScreenManager manager) : base(manager) { }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button");

        grassButton = new SimpleButton(buttonTexture, "Grass",
            new Vector2(800, 400), new Vector2(300, 100));

        stoneButton = new SimpleButton(buttonTexture, "Stone",
            new Vector2(800, 550), new Vector2(300, 100));

        devButton = new SimpleButton(buttonTexture, font, "Dev",
            new Vector2(800, 700), new Vector2(300, 100));

        grassButton.OnClick = () =>
        {
            manager.SetScreen(new LevelSelectScreen(manager, GrassLevels.All));
        };

        stoneButton.OnClick = () =>
        {
            manager.SetScreen(new LevelSelectScreen(manager, GhostLevels.All));
        };

        devButton.OnClick = () =>
        {
            manager.SetScreen(new LevelSelectScreen(manager, DevLevels.All));
        };

        // Play theme music
        AudioManager.Instance.PlayBackgroundMusic("WhatCloudsAreMadeOf");
    }

    public override void Update(GameTime gameTime)
    {
        grassButton.Update(gameTime);
        stoneButton.Update(gameTime);
        devButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);

        grassButton.Draw(spriteBatch);
        stoneButton.Draw(spriteBatch);
        devButton.Draw(spriteBatch);
    }
}
