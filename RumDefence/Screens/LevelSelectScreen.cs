using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace RumDefence;

public class LevelSelectScreen : Screen
{
    private List<Level> levels;
    private List<LevelButton> buttons = new();

    private int currentPage = 0;
    private int levelsPerPage = 4;

    private SpriteFont font;

    private SimpleButton backButton;
    private Texture2D buttonTexture;
    private float elapsedSeconds;

    public LevelSelectScreen(ScreenManager manager, List<Level> levels) : base(manager)
    {
        this.levels = levels;
    }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button");

        buttons.Clear();

        backButton = new SimpleButton(buttonTexture, "Back",
            new Vector2(20, 20), new Vector2(200, 80));

        backButton.OnClick = () =>
        {
            manager.SetScreen(new ThemeSelectScreen(manager));
        };

        for (int i = 0; i < levels.Count; i++)
        {
            var btn = new LevelButton(levels[i]);

            int index = i;

            btn.OnClick = () =>
            {
                if (levels[index].IsUnlocked)
                {
                    manager.SetScreen(new GameScreen(manager, levels[index], levels));
                }
            };

            buttons.Add(btn);
        }

        // Play theme music
        AudioManager.Instance.PlayBackgroundMusic();
    }

    public override void Update(GameTime gameTime)
    {
        elapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (InputManager.Instance.IsActionJustPressed("LevelNext"))
            currentPage = MathHelper.Clamp(currentPage + 1, 0, (levels.Count - 1) / levelsPerPage);

        if (InputManager.Instance.IsActionJustPressed("LevelPrev"))
            currentPage = MathHelper.Clamp(currentPage - 1, 0, (levels.Count - 1) / levelsPerPage);

        int startIndex = currentPage * levelsPerPage;

        for (int i = 0; i < levelsPerPage; i++)
        {
            int levelIndex = startIndex + i;
            if (levelIndex >= levels.Count) break;

            int row = i / 2;
            int col = i % 2;

            Rectangle rect = GetLevelRect(row, col);

            buttons[levelIndex].SetBounds(rect);
            buttons[levelIndex].Update(gameTime);
        }

        backButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);

        LoadingSplashScreen.DrawBackground(spriteBatch, RumGame.VirtualWidth, RumGame.VirtualHeight, elapsedSeconds);
        LoadingSplashScreen.DrawWater(spriteBatch, RumGame.VirtualWidth, RumGame.VirtualHeight, elapsedSeconds);

        int startIndex = currentPage * levelsPerPage;

        for (int i = 0; i < levelsPerPage; i++)
        {
            int levelIndex = startIndex + i;
            if (levelIndex >= levels.Count) break;

            int row = i / 2;
            int col = i % 2;

            Rectangle rect = GetLevelRect(row, col);

            buttons[levelIndex].Draw(spriteBatch);
            DrawLevelScore(spriteBatch, levels[levelIndex], rect);
        }

        backButton.Draw(spriteBatch);
    }

    private Rectangle GetLevelRect(int row, int col)
    {
        int width = 400;
        int height = 300;

        int startX = 400;
        int startY = 250;

        int spacingX = 550;
        int spacingY = 400;

        return new Rectangle(
            startX + col * spacingX,
            startY + row * spacingY,
            width,
            height
        );
    }

    private void DrawLevelScore(SpriteBatch spriteBatch, Level level, Rectangle rect)
    {
        var score = SaveManager.GetLevelScore(level);

        string coinsText;
        string wavesText;

        if (score == null)
        {
            coinsText = "Best Coins: -";
            wavesText = "Best Waves: -";
        }
        else
        {
            coinsText = $"Best Coins: {score.BestCoins}";
            wavesText = $"Best Waves: {score.BestWaves}";
        }

        var coinsSize = font.MeasureString(coinsText);
        var wavesSize = font.MeasureString(wavesText);

        Vector2 coinsPosition = new Vector2(
            rect.Center.X - coinsSize.X / 2,
            rect.Bottom + 10
        );

        Vector2 wavesPosition = new Vector2(
            rect.Center.X - wavesSize.X / 2,
            rect.Bottom + 45
        );

        spriteBatch.DrawString(font, coinsText, coinsPosition, Primitives.FontColor);
        spriteBatch.DrawString(font, wavesText, wavesPosition, Primitives.FontColor);
    }
}
