using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;

namespace RumDefence;

public class PauseScreen : Screen
{
    private Screen previousScreen;

    private Texture2D panelTexture;
    private Texture2D buttonTexture;
    private SpriteFont font;

    private Button resumeButton;
    private Button settingsButton;
    private Button menuButton;

    private Rectangle panelRect;

    private Texture2D pixel;

    public PauseScreen(ScreenManager manager, Screen previous) : base(manager)
    {
        previousScreen = previous;
    }

    private Texture2D GetPixel(SpriteBatch spriteBatch)
    {
        if (pixel == null)
        {
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }
        return pixel;
    }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");

        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel_blue");
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button_blue");

        panelRect = new Rectangle(500, 200, 900, 700);

        resumeButton = new Button(buttonTexture, font, "Resume", new Vector2(800, 300), new Vector2(300, 100));
        settingsButton = new Button(buttonTexture, font, "Settings", new Vector2(800, 450), new Vector2(300, 100));
        menuButton = new Button(buttonTexture, font, "Main Menu", new Vector2(800, 600), new Vector2(300, 100));

        resumeButton.OnClick = () =>
        {
            manager.SetScreen(previousScreen);
        };

        settingsButton.OnClick = () =>
        {
            manager.SetScreen(new SettingsScreen(manager, this));
        };

        menuButton.OnClick = () =>
        {
            manager.SetScreen(new ConfirmScreen(
                manager,
                this,
                "Are you sure you want to return to the main menu?",
                () => manager.SetScreen(new MainMenuScreen(manager))
            ));
        };
    }

    public override void Update(GameTime gameTime)
    {
        resumeButton.Update(gameTime);
        settingsButton.Update(gameTime);
        menuButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        previousScreen.Draw(spriteBatch);

        spriteBatch.Begin();

        spriteBatch.Draw(GetPixel(spriteBatch),
            new Rectangle(0, 0, RumGame.VirtualWidth, RumGame.VirtualHeight),
            Color.Black * 0.5f);

        spriteBatch.Draw(panelTexture, panelRect, Color.White);

        resumeButton.Draw(spriteBatch);
        settingsButton.Draw(spriteBatch);
        menuButton.Draw(spriteBatch);

        spriteBatch.End();
    }
}