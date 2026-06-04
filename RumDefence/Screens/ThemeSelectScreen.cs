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

    private float elapsedSeconds;

    public ThemeSelectScreen(ScreenManager manager) : base(manager) { }

    public override void Load()
    {
        grassButton = new SimpleButton(Primitives.ButtonTexture, "Grass",
            new Vector2(800, 400), new Vector2(300, 100));

        stoneButton = new SimpleButton(Primitives.ButtonTexture, "Stone",
            new Vector2(800, 550), new Vector2(300, 100));

        backButton = new SimpleButton(Primitives.ButtonTexture, "Back",
            new Vector2(20, 20), new Vector2(200, 80));


        bool devMode = Environment.GetEnvironmentVariable("RUM_DEV") == "true";

        if (devMode)
        {
            devButton = new SimpleButton(Primitives.ButtonTexture, "Dev",
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
        AudioManager.Instance.PlayBackgroundMusic();
    }

    public override void Update(GameTime gameTime)
    {
        elapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
        grassButton.Update(gameTime);
        stoneButton.Update(gameTime);
        devButton?.Update(gameTime);
        backButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);


        LoadingSplashScreen.DrawBackground(spriteBatch, RumGame.VirtualWidth, RumGame.VirtualHeight, elapsedSeconds);
        LoadingSplashScreen.DrawWater(spriteBatch, RumGame.VirtualWidth, RumGame.VirtualHeight, elapsedSeconds);

        grassButton.Draw(spriteBatch);
        stoneButton.Draw(spriteBatch);
        devButton?.Draw(spriteBatch);
        backButton.Draw(spriteBatch);
    }
}
