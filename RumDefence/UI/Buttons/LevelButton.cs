using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence;

public class LevelButton : Button
{
    private Level level;

    public LevelButton(Level level)
    {
        this.level = level;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (!level.IsUnlocked)
            return;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Rectangle panel = new Rectangle(
            bounds.X + 4,
            bounds.Y + 4,
            bounds.Width - 8,
            bounds.Height - 8
        );

        NineSlice.Draw(spriteBatch, Primitives.ButtonTexture, panel, new Rectangle(
            0,
            0,
            Primitives.ButtonTexture.Width,
            Primitives.ButtonTexture.Height
        ), 20, level.IsUnlocked ? Color.White : Color.Brown * 0.85f);

        int mapWidth = 340;
        int mapHeight = 200;

        Rectangle mapRect = new Rectangle(
            panel.X + (panel.Width - mapWidth) / 2,
            panel.Y + (panel.Height - mapHeight) / 2 + 20,
            mapWidth,
            mapHeight
        );

        MiniMapRenderer.Draw(spriteBatch, level, mapRect, level.IsUnlocked);
        spriteBatch.Draw(
            Primitives.Pixel,
            mapRect,
            Color.White * 0.4f
        );
        DrawLevelScore(spriteBatch, level, mapRect);

        var labelColor = isPressed ? Color.White : Primitives.FontColor;

        spriteBatch.DrawString(
            Primitives.Font,
            $"LEVEL {level.Id}",
            new Vector2(mapRect.X, panel.Y + 4),
            labelColor
        );

    }
    private void DrawLevelScore(SpriteBatch spriteBatch, Level level, Rectangle rect)
    {
        var score = SaveManager.GetLevelScore(level);
        var levelScore = "Best Score\n";

        if (score == null)
        {
            levelScore += "Coins: -\n";
            levelScore += "Waves: -\n";
        }
        else
        {
            levelScore += $"Coins: {score.BestCoins}\n";
            levelScore += $"Waves: {score.BestWaves}\n";
        }
        spriteBatch.DrawString(Primitives.Font, levelScore, new Vector2(
            rect.X + 4,
            rect.Top + 1
        ), Color.SaddleBrown);
    }
    protected override bool IsClickable()
    {
        return level.IsUnlocked;
    }
}
