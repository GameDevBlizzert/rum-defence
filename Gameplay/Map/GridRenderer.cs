using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence;

public class GridRenderer
{
    private int tileSize;
    private ITileTheme theme;

    public GridRenderer(ITileTheme theme)
    {
        this.theme = theme;
    }

    public void Draw(Grid grid, SpriteBatch spriteBatch)
    {
        int screenWidth = RumGame.Instance.GraphicsDevice.Viewport.Width;
        int screenHeight = RumGame.Instance.GraphicsDevice.Viewport.Height;

        int padding = 100;

        int tileWidth = (screenWidth - padding) / grid.Width;
        int tileHeight = (screenHeight - padding) / grid.Height;

        tileSize = Math.Min(tileWidth, tileHeight);

        int gridWidth = grid.Width * tileSize;
        int gridHeight = grid.Height * tileSize;

        Vector2 offset = new(
            (screenWidth - gridWidth) / 2,
            (screenHeight - gridHeight) / 2
        );

        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                var pos = offset + new Vector2(x * tileSize, y * tileSize);

                var texture = theme.GetTexture(grid.Tiles[y, x], x, y);

                if (texture != null)
                {
                    spriteBatch.Draw(texture,
                        new Rectangle((int)pos.X, (int)pos.Y, tileSize, tileSize),
                        Color.White);
                }
            }
        }
    }
}