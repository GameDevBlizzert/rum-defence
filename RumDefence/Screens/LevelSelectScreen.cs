using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RumDefence.UI.Box.Components;
using System.Collections.Generic;

namespace RumDefence;

public class LevelSelectScreen : Screen
{
    private List<Level> levels;
    private List<LevelButton> buttons = new();

    private int currentPage = 0;
    private int levelsPerPage = 4;

    private ButtonBox backButton;
    private float elapsedSeconds;

    public LevelSelectScreen(ScreenManager manager, List<Level> levels) : base(manager)
    {
        this.levels = levels;
    }

    public override void Load()
    {
        buttons.Clear();

        backButton = new ButtonBox(Primitives.ButtonTexture, "Back");
        backButton.Arrange(new Rectangle(20, 20, 200, 80));

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

            buttons[levelIndex].Arrange(rect);
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

            buttons[levelIndex].Draw(spriteBatch);
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


}
