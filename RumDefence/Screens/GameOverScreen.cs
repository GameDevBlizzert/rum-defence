using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box;
using System.Collections.Generic;

namespace RumDefence;

public class GameOverScreen : Screen
{
    private ButtonBox retryButton;
    private ButtonBox menuButton;
    private ButtonBox nextLevelButton;

    private Box panel;

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
        var buttonSize = new Vector2(360, 100);

        retryButton = new ButtonBox(Primitives.ButtonTexture, "Retry", size: buttonSize);
        menuButton = new ButtonBox(Primitives.ButtonTexture, "Menu", size: buttonSize);
        nextLevelButton = new ButtonBox(Primitives.ButtonTexture, "Next Level", size: buttonSize);

        int panelWidth = 700;
        int panelHeight = 700;
        var panelRect = new Rectangle(
            screenWidth / 2 - panelWidth / 2,
            screenHeight / 2 - panelHeight / 2,
            panelWidth,
            panelHeight
        );

        panel = new Box
        {
            Direction = Direction.Row,
            AlignX = Align.Center,
            AlignY = Align.Center,
            Gap = 20,
        };
        panel.AddBackground(new ImageBox(Primitives.PanelTexture));

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

        var title = isWin ? "YOU WIN!" : "GAME OVER";
        panel.Add(new TextItem(title, scale: 1.5f));
        panel.Add(new TextItem($"Waves: {wavesSurvived}"));
        panel.Add(new TextItem($"Coins: {coins}"));

        if (isWin)
            panel.Add(nextLevelButton);
        panel.Add(retryButton);
        panel.Add(menuButton);

        panel.Arrange(panelRect);
    }

    public override void Update(GameTime gameTime)
    {
        panel.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        previousScreen.Draw(spriteBatch);

        spriteBatch.Draw(
            Primitives.Pixel,
            new Rectangle(0, 0, screenWidth, screenHeight),
            Color.Black * 0.6f
        );

        panel.Draw(spriteBatch);
    }
}
