using Microsoft.Xna.Framework;

namespace RumDefence;

public class Grid
{
    public int[,] Tiles;

    public int Width => Tiles.GetLength(1);
    public int Height => Tiles.GetLength(0);

    public int TileSize { get; set; }
    public Vector2 Offset { get; set; }

    public Grid(int[,] map)
    {
        Tiles = map;
    }

    public Vector2 GridToWorld(Point gridPos)
    {
        return Offset + new Vector2(
            gridPos.X * TileSize + TileSize / 2f,
            gridPos.Y * TileSize + TileSize / 2f
        );
    }
}