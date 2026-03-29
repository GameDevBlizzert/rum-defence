using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence;

public class LevelButton : Button
{
    private Level level;

    private Texture2D pixel;
    private SpriteFont font;

    public LevelButton(Level level, Texture2D pixel, SpriteFont font)
    {
        this.level = level;
        this.pixel = pixel;
        this.font = font;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (!level.IsUnlocked)
            return;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Color borderColor = isHovering ? Color.White : Color.Gray;

        Color panelColor = isHovering
            ? new Color(220, 220, 220)
            : new Color(180, 180, 180);

        spriteBatch.Draw(pixel, bounds, borderColor);

        Rectangle panel = new Rectangle(
            bounds.X + 4,
            bounds.Y + 4,
            bounds.Width - 8,
            bounds.Height - 8
        );

        spriteBatch.Draw(pixel, panel, panelColor);

        int mapWidth = 340;
        int mapHeight = 200;

        Rectangle mapRect = new Rectangle(
            panel.X + (panel.Width - mapWidth) / 2,
            panel.Y + (panel.Height - mapHeight) / 2 + 10,
            mapWidth,
            mapHeight
        );

        MiniMapRenderer.Draw(spriteBatch, level, mapRect, level.IsUnlocked);

        spriteBatch.DrawString(
            font,
            $"LEVEL {level.Id}",
            new Vector2(bounds.X + 10, bounds.Y + 5),
            Color.White
        );

        if (!level.IsUnlocked)
        {
            spriteBatch.Draw(pixel, bounds, Color.Black * 0.5f);
        }
    }

    protected override bool IsClickable()
    {
        return level.IsUnlocked;
    }
}