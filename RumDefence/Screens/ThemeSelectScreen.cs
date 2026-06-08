using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.Gameplay.Levels.Dev;
using RumDefence.Gameplay.Levels.Ghost;
using RumDefence.Levels.Ghost;
using RumDefence.Levels.Grass;
using RumDefence.UI.Box;

namespace RumDefence;

public class ThemeSelectScreen : Screen
{
    private ButtonBox grassButton;
    private ButtonBox stoneButton;
    private ButtonBox devButton;
    private ButtonBox backButton;

    private float elapsedSeconds;

    public ThemeSelectScreen(ScreenManager manager) : base(manager) { }

    public override void Load()
    {
        grassButton = new ButtonBox(Primitives.ButtonTexture, "Grass");
        grassButton.Arrange(new Rectangle(800, 400, 300, 100));

        stoneButton = new ButtonBox(Primitives.ButtonTexture, "Stone");
        stoneButton.Arrange(new Rectangle(800, 550, 300, 100));

        backButton = new ButtonBox(Primitives.ButtonTexture, "Back");
        backButton.Arrange(new Rectangle(20, 20, 200, 80));


        bool devMode = Environment.GetEnvironmentVariable("RUM_DEV") == "true";

        if (devMode)
        {
            devButton = new ButtonBox(Primitives.ButtonTexture, "Dev");
            devButton.Arrange(new Rectangle(800, 700, 300, 100));
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
