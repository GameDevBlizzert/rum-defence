using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class MainMenuScreen : Screen
{
    private SpriteFont font;
    private SpriteFont titleFont;

    private SimpleButton levelsButton;
    private SimpleButton settingsButton;
    private SimpleButton quitButton;

    private Rectangle panelRect;

    public MainMenuScreen(ScreenManager manager) : base(manager) { }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");
        titleFont = content.Load<SpriteFont>("Fonts/KenneyFuture");

        panelRect = new Rectangle(500, 200, 900, 700);

        // Center buttons horizontally
        int buttonWidth = 400;
        int buttonCenterX = (RumGame.VirtualWidth - buttonWidth) / 2;

        levelsButton = new SimpleButton(font, "Levels", new Vector2(buttonCenterX, 350), new Vector2(buttonWidth, 120));
        settingsButton = new SimpleButton(font, "Settings", new Vector2(buttonCenterX, 520), new Vector2(buttonWidth, 120));
        quitButton = new SimpleButton(font, "Quit", new Vector2(buttonCenterX, 690), new Vector2(buttonWidth, 120));

        levelsButton.OnClick = () =>
        {
            manager.SetScreen(new ThemeSelectScreen(manager));
        };

        settingsButton.OnClick = () =>
        {
            System.Diagnostics.Debug.WriteLine("Settings clicked");
        };

        quitButton.OnClick = () =>
        {
            manager.SetScreen(new ConfirmScreen(
                manager,
                this,
                "Are you sure you want to quit?",
                () => RumGame.Instance.Exit()
            ));
        };

        // Play main menu theme music
        AudioManager.Instance.PlayBackgroundMusic("WhatCloudsAreMadeOf");
    }

    public override void Update(GameTime gameTime)
    {
        levelsButton.Update(gameTime);
        settingsButton.Update(gameTime);
        quitButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(new Color(30, 30, 30));

        // Draw title
        var titleSize = titleFont.MeasureString("Rum Defence");
        var titleX = (RumGame.VirtualWidth - titleSize.X) / 2;
        spriteBatch.DrawString(titleFont, "Rum Defence", new Vector2(titleX, 80), Color.White);

        levelsButton.Draw(spriteBatch);
        settingsButton.Draw(spriteBatch);
        quitButton.Draw(spriteBatch);
    }
}
