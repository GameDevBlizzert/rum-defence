using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence;

public class GameOverScreen : Screen
{
    private Texture2D panelTexture;
    private Texture2D buttonTexture;

    private SimpleButton retryButton;
    private SimpleButton menuButton;
    private SimpleButton nextLevelButton;

    private Level level;
    private List<Level> levelSet;
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
        List<Level> levelSet,
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
        this.levelSet = levelSet;
    }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel");
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button");

        Vector2 buttonSize = new Vector2(360, 100);

        retryButton = new SimpleButton(buttonTexture, "Retry", Vector2.Zero, buttonSize);
        menuButton = new SimpleButton(buttonTexture, "Menu", Vector2.Zero, buttonSize);
        nextLevelButton = new SimpleButton(buttonTexture, "Next Level", Vector2.Zero, buttonSize);

        if (isWin)
        {
            SaveManager.UnlockLevel(level);
            SaveManager.UnlockNextLevel(levelSet, level);
            SaveManager.SaveWinScore(level, coins, wavesSurvived);

            int currentIndex = levelSet.FindIndex(l => l.Id == level.Id);

            if (currentIndex >= 0 && currentIndex < levelSet.Count - 1)
            {
                Level nextLevel = levelSet[currentIndex + 1];
                SaveManager.UnlockLevel(nextLevel);
            }
        }

        retryButton.OnClick = () =>
        {
            manager.SetScreen(new GameScreen(manager, level, levelSet));
        };

        menuButton.OnClick = () =>
        {
            manager.SetScreen(new MainMenuScreen(manager));
        };

        nextLevelButton.OnClick = () =>
        {
            if (!isWin)
                return;

            int currentIndex = levelSet.FindIndex(l => l.Id == level.Id);

            if (currentIndex >= 0 && currentIndex < levelSet.Count - 1)
            {
                Level nextLevel = levelSet[currentIndex + 1];
                SaveManager.UnlockLevel(nextLevel);
                manager.SetScreen(new GameScreen(manager, nextLevel, levelSet));
            }
            else
            {
                manager.SetScreen(new MainMenuScreen(manager));
            }
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

        Vector2 buttonSize = new Vector2(360, 100);
        float centerX = panelRect.Center.X;

        if (isWin)
        {
            nextLevelButton.SetBounds(new Rectangle(
                (int)(centerX - buttonSize.X / 2),
                panelRect.Y + 300,
                (int)buttonSize.X,
                (int)buttonSize.Y
            ));

            retryButton.SetBounds(new Rectangle(
                (int)(centerX - buttonSize.X / 2),
                panelRect.Y + 420,
                (int)buttonSize.X,
                (int)buttonSize.Y
            ));

            menuButton.SetBounds(new Rectangle(
                (int)(centerX - buttonSize.X / 2),
                panelRect.Y + 540,
                (int)buttonSize.X,
                (int)buttonSize.Y
            ));

            nextLevelButton.Update(gameTime);
        }
        else
        {
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
        }

        retryButton.Update(gameTime);
        menuButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        previousScreen.Draw(spriteBatch);

        spriteBatch.Draw(
            Primitives.Pixel,
            new Rectangle(0, 0, screenWidth, screenHeight),
            Color.Black * 0.6f
        );

        int panelWidth = 700;
        int panelHeight = 700;

        var panelRect = new Rectangle(
            screenWidth / 2 - panelWidth / 2,
            screenHeight / 2 - panelHeight / 2,
            panelWidth,
            panelHeight
        );

        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        var title = isWin ? "YOU WIN!" : "GAME OVER";
        var titleSize = Primitives.Font.MeasureString(title);

        spriteBatch.DrawString(
            Primitives.Font,
            title,
            new Vector2(panelRect.Center.X - titleSize.X / 2, panelRect.Y + 40),
            Primitives.FontColor
        );

        DrawCenteredText(spriteBatch, $"Waves: {wavesSurvived}", panelRect.Center.X, panelRect.Y + 140);
        DrawCenteredText(spriteBatch, $"Coins: {coins}", panelRect.Center.X, panelRect.Y + 190);

        if (isWin)
            nextLevelButton.Draw(spriteBatch);

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