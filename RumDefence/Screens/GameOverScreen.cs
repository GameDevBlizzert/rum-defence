using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class GameOverScreen : Screen
{
    private Texture2D panelTexture;
    private Texture2D buttonTexture;

    private SimpleButton retryButton;
    private SimpleButton menuButton;

    private Level level;
    private bool isWin;

    private int wavesSurvived;
    private int coins;

    private GameScreen previousScreen;

    private int screenWidth = RumGame.VirtualWidth;
    private int screenHeight = RumGame.VirtualHeight;

    public GameOverScreen(
        ScreenManager manager,
        GameScreen previousScreen,
        Level level,
        bool isWin,
        int wavesSurvived,
        int coins
    ) : base(manager)
    {
        this.previousScreen = previousScreen;
        this.level = level;
        this.isWin = isWin;
        this.wavesSurvived = wavesSurvived;
        this.coins = coins;
    }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel");
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button");

        Vector2 buttonSize = new Vector2(300, 100);

        retryButton = new SimpleButton(buttonTexture, "Retry", Vector2.Zero, buttonSize);
        menuButton = new SimpleButton(buttonTexture, "Menu", Vector2.Zero, buttonSize);

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
        int panelWidth = 700;
        int panelHeight = 700;

        var panelRect = new Rectangle(
            screenWidth / 2 - panelWidth / 2,
            screenHeight / 2 - panelHeight / 2,
            panelWidth,
            panelHeight
        );

        Vector2 buttonSize = new Vector2(300, 100);
        float centerX = panelRect.Center.X;

        retryButton.SetBounds(new Rectangle(
            (int)(centerX - buttonSize.X / 2),
            panelRect.Y + 320,
            (int)buttonSize.X,
            (int)buttonSize.Y
        ));

        menuButton.SetBounds(new Rectangle(
            (int)(centerX - buttonSize.X / 2),
            panelRect.Y + 440,
            (int)buttonSize.X,
            (int)buttonSize.Y
        ));

        retryButton.Update(gameTime);
        menuButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // 1. game erachter
        previousScreen.Draw(spriteBatch);

        // 2. overlay
        spriteBatch.Draw(
            Primitives.Pixel,
            new Rectangle(0, 0, screenWidth, screenHeight),
            Color.Black * 0.6f
        );

        // 3. panel
        int panelWidth = 700;
        int panelHeight = 700;

        var panelRect = new Rectangle(
            screenWidth / 2 - panelWidth / 2,
            screenHeight / 2 - panelHeight / 2,
            panelWidth,
            panelHeight
        );

        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        // 4. titel
        var title = isWin ? "YOU WIN!" : "GAME OVER";
        var titleSize = Primitives.Font.MeasureString(title);

        spriteBatch.DrawString(
            Primitives.Font,
            title,
            new Vector2(panelRect.Center.X - titleSize.X / 2, panelRect.Y + 40),
            Primitives.FontColor
        );

        // 5. stats
        DrawCenteredText(spriteBatch, $"Waves: {wavesSurvived}", panelRect.Center.X, panelRect.Y + 140);
        DrawCenteredText(spriteBatch, $"Coins: {coins}", panelRect.Center.X, panelRect.Y + 190);

        // 6. buttons
        retryButton.Draw(spriteBatch);
        menuButton.Draw(spriteBatch);
    }

    private void DrawCenteredText(SpriteBatch spriteBatch, string text, float centerX, float y)
    {
        var size = Primitives.Font.MeasureString(text);

        spriteBatch.DrawString(
            Primitives.Font,
            text,
            new Vector2(centerX - size.X / 2, y),
            Primitives.FontColor
        );
    }
}