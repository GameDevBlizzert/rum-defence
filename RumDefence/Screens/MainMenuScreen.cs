using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class MainMenuScreen : Screen
{
    private Texture2D panelTexture;
    private Texture2D buttonTexture;

    private SpriteFont font;

    private SimpleButton levelsButton;
    private SimpleButton settingsButton;
    private SimpleButton quitButton;

    private Rectangle panelRect;

    public MainMenuScreen(ScreenManager manager) : base(manager) { }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");

        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel_blue");
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button_blue");

        panelRect = new Rectangle(500, 200, 900, 700);

        levelsButton = new SimpleButton(buttonTexture, font, "Levels", new Vector2(800, 300), new Vector2(300, 100));
        settingsButton = new SimpleButton(buttonTexture, font, "Settings", new Vector2(800, 450), new Vector2(300, 100));
        quitButton = new SimpleButton(buttonTexture, font, "Quit", new Vector2(800, 600), new Vector2(300, 100));

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
    }

    public override void Update(GameTime gameTime)
    {
        levelsButton.Update(gameTime);
        settingsButton.Update(gameTime);
        quitButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);

        spriteBatch.Draw(panelTexture, panelRect, Color.White);

        levelsButton.Draw(spriteBatch);
        settingsButton.Draw(spriteBatch);
        quitButton.Draw(spriteBatch);
    }
}