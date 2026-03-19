namespace RumDefence;

public class Grid
{
    public int[,] Tiles;

    public int Width => Tiles.GetLength(1);
    public int Height => Tiles.GetLength(0);

    public Grid(int[,] level)
    {
        Tiles = level;
    }
}