public static class TileRules
{
    public const int Water = 0;
    public const int Land = 1;

    public static bool IsWater(int tile) => tile == Water;
    public static bool IsLand(int tile) => tile == Land;

    public static bool IsCoast(int[,] map, int x, int y)
    {
        if (map[y, x] != Land)
            return false;

        int h = map.GetLength(0);
        int w = map.GetLength(1);

        bool IsWater(int px, int py)
        {
            if (px < 0 || px >= w || py < 0 || py >= h)
                return false;

            return map[py, px] == Water;
        }

        return
            IsWater(x, y - 1) ||
            IsWater(x + 1, y) ||
            IsWater(x - 1, y) ||
            IsWater(x, y + 1);
    }
}