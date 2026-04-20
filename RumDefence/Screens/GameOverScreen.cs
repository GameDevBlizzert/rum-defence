using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class GameOverScreen : Screen
{
    private Texture2D panelTexture;
    private Texture2D buttonTexture;
    private SpriteFont font;

    private SimpleButton retryButton;
    private SimpleButton menuButton;

    private Rectangle panelRect;

    private Level level;
    private bool isWin;

    public GameOverScreen(ScreenManager manager, Level level, bool isWin) : base(manager)
    {
        this.level = level;   
        this.isWin = isWin;  
    }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");
        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel_blue");
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button_blue");

        panelRect = new Rectangle(500, 200, 900, 700);

        retryButton = new SimpleButton(
            buttonTexture,
            font,
            "Retry",
            new Vector2(800, 450),
            new Vector2(300, 100)
        );

        menuButton = new SimpleButton(
            buttonTexture,
            font,
            "Menu",
            new Vector2(800, 600),
            new Vector2(300, 100)
        );

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

        var text = isWin ? "YOU WIN" : "GAME OVER";
        var size = font.MeasureString(text);

        var textPos = new Vector2(
            panelRect.Center.X - size.X / 2,
            panelRect.Y + 100
        );

        spriteBatch.DrawString(font, text, textPos, Color.White);

        retryButton.Draw(spriteBatch);
        menuButton.Draw(spriteBatch);
    }
}