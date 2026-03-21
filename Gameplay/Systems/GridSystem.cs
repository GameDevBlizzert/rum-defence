using Microsoft.Xna.Framework;
using System;

namespace RumDefence;

public static class GridSystem
{
    public static void CalculateLayout(Grid grid)
    {
        int screenWidth = RumGame.VirtualWidth;
        int screenHeight = RumGame.VirtualHeight;

        int padding = screenWidth / 20;

        int tileWidth = (screenWidth - padding) / grid.Width;
        int tileHeight = (screenHeight - padding) / grid.Height;

        int tileSize = Math.Min(tileWidth, tileHeight);

        int gridWidth = grid.Width * tileSize;
        int gridHeight = grid.Height * tileSize;

        Vector2 offset = new(
            (screenWidth - gridWidth) / 2,
            (screenHeight - gridHeight) / 2
        );

        grid.TileSize = tileSize;
        grid.Offset = offset;
    }
}