namespace RumDefence;

public static class TileHelper
{
    // 0 = water
    public static bool IsWater(int tile)
    {
        return tile == 0;
    }

    // alles behalve water = land
    public static bool IsLand(int tile)
    {
        return tile != 0;
    }

    // kust = land tile die NIET het midden is
    // (dus 1–9 behalve 5)
    public static bool IsCoast(int tile)
    {
        return tile != 0 && tile != 5;
    }
}