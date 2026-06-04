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
        ), 20, Color.White * 0.96f);

        int mapWidth = 340;
        int mapHeight = 200;

        Rectangle mapRect = new Rectangle(
            panel.X + (panel.Width - mapWidth) / 2,
            panel.Y + (panel.Height - mapHeight) / 2 + 10,
            mapWidth,
            mapHeight
        );

        MiniMapRenderer.Draw(spriteBatch, level, mapRect, level.IsUnlocked);

        var labelColor = isPressed ? Color.White : Primitives.FontColor;

        spriteBatch.DrawString(
            Primitives.Font,
            $"LEVEL {level.Id}",
            new Vector2(panel.X + (panel.Width - mapWidth) / 2, bounds.Y + 5),
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
