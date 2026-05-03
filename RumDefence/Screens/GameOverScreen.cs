using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class GameOverScreen : Screen
{
    private readonly Level level;

    private Texture2D panelTexture;
    private Texture2D buttonTexture;
    private SpriteFont font;

    private SimpleButton retryButton;
    private SimpleButton menuButton;

    private Rectangle panelRect;

    public GameOverScreen(ScreenManager manager, Level level) : base(manager)
    {
        this.level = level;
    }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");
        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel_blue");
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button_blue");

        panelRect = new Rectangle(500, 250, 900, 600);

        retryButton = new SimpleButton(buttonTexture, font, "Retry", new Vector2(800, 520), new Vector2(300, 100));
        menuButton = new SimpleButton(buttonTexture, font, "Main Menu", new Vector2(800, 650), new Vector2(300, 100));

        AudioManager.Instance.PlaySound("lose");

        retryButton.OnClick = () =>
        {
            manager.SetScreen(new GameScreen(manager, level));
        };

        menuButton.OnClick = () =>
        {
            manager.SetScreen(new MainMenuScreen(manager));
        };
    }

    public override void Update(GameTime gameTime)
    {
        retryButton.Update(gameTime);
        menuButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(Color.Black);

        spriteBatch.Draw(panelTexture, panelRect, Color.White);
        spriteBatch.DrawString(font, "Game Over", new Vector2(820, 360), Color.Black);

        retryButton.Draw(spriteBatch);
        menuButton.Draw(spriteBatch);
    }
}
