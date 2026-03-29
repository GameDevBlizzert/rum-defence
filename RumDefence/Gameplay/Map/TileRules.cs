namespace RumDefence;

public static class TileRules
{
    public const int Water = 0;
    public const int Center = 5;

    public static bool IsWater(int tile) => tile == Water;

    public static bool IsLand(int tile) => tile != Water;

    public static bool IsCoast(int tile) => tile != Water && tile != Center;
}