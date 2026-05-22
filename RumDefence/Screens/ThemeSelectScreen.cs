using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.Gameplay.Levels.Dev;
using RumDefence.Gameplay.Levels.Ghost;
using RumDefence.Levels.Ghost;
using RumDefence.Levels.Grass;

namespace RumDefence;

public class ThemeSelectScreen : Screen
{
    private SimpleButton grassButton;
    private SimpleButton stoneButton;
    private SimpleButton devButton;
    private SimpleButton backButton;

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

        backButton = new SimpleButton(buttonTexture, "Back",
            new Vector2(20, 20), new Vector2(200, 80));


        bool devMode = Environment.GetEnvironmentVariable("RUM_DEV") == "true";

        if (devMode)
        {
            devButton = new SimpleButton(buttonTexture, "Dev",
                new Vector2(800, 700), new Vector2(300, 100));
            devButton.OnClick = () =>
            {
                manager.SetScreen(new LevelSelectScreen(manager, DevLevels.All));
            };
        }

        grassButton.OnClick = () =>
        {
            manager.SetScreen(new LevelSelectScreen(manager, GrassLevels.All));
        };

        stoneButton.OnClick = () =>
        {
            manager.SetScreen(new LevelSelectScreen(manager, GhostLevels.All));
        };

        backButton.OnClick = () =>
        {
            manager.SetScreen(new MainMenuScreen(manager));
        };

        // Play theme music
        AudioManager.Instance.PlayBackgroundMusic("WhatCloudsAreMadeOf");
    }

    public override void Update(GameTime gameTime)
    {
        grassButton.Update(gameTime);
        stoneButton.Update(gameTime);
        devButton?.Update(gameTime);
        backButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);

        grassButton.Draw(spriteBatch);
        stoneButton.Draw(spriteBatch);
        devButton?.Draw(spriteBatch);
        backButton.Draw(spriteBatch);
    }
}
