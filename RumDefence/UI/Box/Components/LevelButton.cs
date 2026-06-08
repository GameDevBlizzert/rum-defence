using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box.Components;

namespace RumDefence;

public class LevelButton : ButtonBox
{
    private Level level;

    public LevelButton(Level level)
    {
        this.level = level;
        AddBackground(new ImageBox(Primitives.ButtonTexture));
        BaseTint = level.IsUnlocked ? Color.White : Color.Brown * 0.85f;
    }

    protected override bool IsClickable() => level.IsUnlocked;

    public override void DrawBox(SpriteBatch spriteBatch)
    {
        base.DrawBox(spriteBatch);

        Rectangle panel = new Rectangle(
            Slot.X + 4,
            Slot.Y + 4,
            Slot.Width - 8,
            Slot.Height - 8
        );

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

        spriteBatch.DrawString(
            Primitives.Font,
            $"LEVEL {level.Id}",
            new Vector2(mapRect.X, panel.Y + 4),
            Primitives.FontColor
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
}
