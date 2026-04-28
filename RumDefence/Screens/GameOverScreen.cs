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

    private int wavesSurvived;
    private int coins;
    private int lives;

    public GameOverScreen(
        ScreenManager manager,
        Level level,
        bool isWin,
        int wavesSurvived,
        int coins,
        int lives
    ) : base(manager)
    {
        this.level = level;
        this.isWin = isWin;
        this.wavesSurvived = wavesSurvived;
        this.coins = coins;
        this.lives = lives;
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
            new Vector2(panelRect.Center.X - 150, panelRect.Y + 450),
            new Vector2(300, 100)
        );

        menuButton = new SimpleButton(
            buttonTexture,
            font,
            "Menu",
            new Vector2(panelRect.Center.X - 150, panelRect.Y + 580),
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

        // panel
        spriteBatch.Draw(panelTexture, panelRect, Color.White);

        // titel
        var title = isWin ? "YOU WIN!" : "GAME OVER";
        var titleSize = font.MeasureString(title);

        var titlePos = new Vector2(
            panelRect.Center.X - titleSize.X / 2,
            panelRect.Y + 80
        );

        spriteBatch.DrawString(font, title, titlePos, Color.White);

        // stats
        int startY = panelRect.Y + 220;
        int spacing = 50;

        DrawCenteredText(spriteBatch, $"Waves survived: {wavesSurvived}", startY);
        DrawCenteredText(spriteBatch, $"Coins: {coins}", startY + spacing);
        DrawCenteredText(spriteBatch, $"Lives left: {lives}", startY + spacing * 2);

        // buttons
        retryButton.Draw(spriteBatch);
        menuButton.Draw(spriteBatch);
    }

    private void DrawCenteredText(SpriteBatch spriteBatch, string text, float y)
    {
        var size = font.MeasureString(text);

        var pos = new Vector2(
            panelRect.Center.X - size.X / 2,
            y
        );

        spriteBatch.DrawString(font, text, pos, Color.White);
    }
}