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
        Color borderColor = isPressed
            ? new Color(25, 25, 25)
            : isHovering
                ? new Color(255, 255, 255)
                : Color.Gray;

        Color panelColor = isPressed
            ? new Color(35, 35, 35)
            : isHovering
                ? new Color(255, 244, 190)
                : new Color(180, 180, 180);

        spriteBatch.Draw(Primitives.Pixel, bounds, borderColor);

        Rectangle panel = new Rectangle(
            bounds.X + 4,
            bounds.Y + 4,
            bounds.Width - 8,
            bounds.Height - 8
        );

        spriteBatch.Draw(Primitives.Pixel, panel, panelColor);

        int mapWidth = 340;
        int mapHeight = 200;

        Rectangle mapRect = new Rectangle(
            panel.X + (panel.Width - mapWidth) / 2,
            panel.Y + (panel.Height - mapHeight) / 2 + 10,
            mapWidth,
            mapHeight
        );

        MiniMapRenderer.Draw(spriteBatch, level, mapRect, level.IsUnlocked);

        var labelColor = isHovering && !isPressed ? Color.Black : Primitives.FontColor;

        spriteBatch.DrawString(
            Primitives.Font,
            $"LEVEL {level.Id}",
            new Vector2(bounds.X + 10, bounds.Y + 5),
            labelColor
        );

        if (!level.IsUnlocked)
        {
            spriteBatch.Draw(Primitives.Pixel, bounds, Color.Black * 0.5f);
        }
    }

    protected override bool IsClickable()
    {
        return level.IsUnlocked;
    }
}
