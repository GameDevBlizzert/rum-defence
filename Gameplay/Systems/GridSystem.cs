using Microsoft.Xna.Framework;
using System;

namespace RumDefence;

public static class GridSystem
{
    public static void CalculateLayout(Grid grid)
    {
        int screenWidth = RumGame.VirtualWidth;
        int screenHeight = RumGame.VirtualHeight;

        int gridWidth = grid.Width;
        int gridHeight = grid.Height;

        float tileWidth = (float)screenWidth / gridWidth;
        float tileHeight = (float)screenHeight / gridHeight;

        int tileSize = (int)Math.Min(tileWidth, tileHeight);

        grid.TileSize = tileSize;

        int totalWidth = grid.Width * tileSize;
        int totalHeight = grid.Height * tileSize;

        grid.Offset = new Vector2(
            (screenWidth - totalWidth) / 2,
            (screenHeight - totalHeight) / 2
        );
    }
}