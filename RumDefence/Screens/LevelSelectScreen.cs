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

    private Texture2D pixel;
    private SpriteFont font;

    public LevelSelectScreen(ScreenManager manager, List<Level> levels) : base(manager)
    {
        this.levels = levels;
    }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");

        pixel = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        buttons.Clear();

        for (int i = 0; i < levels.Count; i++)
        {
            var btn = new LevelButton(levels[i], pixel, font);

            int index = i;

            btn.OnClick = () =>
            {
                if (levels[index].IsUnlocked)
                {
                    manager.SetScreen(new GameScreen(manager, levels[index]));
                }
            };

            buttons.Add(btn);
        }

        // Play theme music
        AudioManager.Instance.PlayBackgroundMusic("WhatCloudsAreMadeOf");
    }

    public override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.D))
            currentPage = MathHelper.Clamp(currentPage + 1, 0, (levels.Count - 1) / levelsPerPage);

        if (keyboard.IsKeyDown(Keys.A))
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
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);

        int startIndex = currentPage * levelsPerPage;

        for (int i = 0; i < levelsPerPage; i++)
        {
            int levelIndex = startIndex + i;
            if (levelIndex >= levels.Count) break;

            buttons[levelIndex].Draw(spriteBatch);
        }
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
