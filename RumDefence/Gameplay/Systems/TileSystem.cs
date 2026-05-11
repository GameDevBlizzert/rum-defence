using Microsoft.Xna.Framework;

namespace RumDefence;

public static class TileSystem
{
    public static int GetTile(int[,] map, Point rumTile, int x, int y)
    {
        int mask = GetMask(map, rumTile, x, y);
        return MapMaskToTile(map, rumTile, x, y, mask);
    }

    private static bool IsLandSafe(int[,] map, Point rumTile, int x, int y)
    {
        int h = map.GetLength(0);
        int w = map.GetLength(1);

        if (x < 0 || x >= w || y < 0 || y >= h)
            return true;

        if (rumTile == new Point(x, y))
            return true;

        return map[y, x] == 1;
    }

    private static bool IsLandDiag(int[,] map, Point rumTile, int x, int y)
    {
        int h = map.GetLength(0);
        int w = map.GetLength(1);

        if (x < 0 || x >= w || y < 0 || y >= h)
            return true;

        if (rumTile == new Point(x, y))
            return true;

        return map[y, x] == 1;
    }

    private static int GetMask(int[,] map, Point rumTile, int x, int y)
    {
        int mask = 0;

        if (IsLandSafe(map, rumTile, x, y - 1)) mask |= 1;
        if (IsLandSafe(map, rumTile, x + 1, y)) mask |= 2;
        if (IsLandSafe(map, rumTile, x - 1, y)) mask |= 4;
        if (IsLandSafe(map, rumTile, x, y + 1)) mask |= 8;

        return mask;
    }

    private static int MapMaskToTile(int[,] map, Point rumTile, int x, int y, int mask)
    {
        bool top = (mask & 1) != 0;
        bool right = (mask & 2) != 0;
        bool left = (mask & 4) != 0;
        bool bottom = (mask & 8) != 0;

        bool diagTL = IsLandDiag(map, rumTile, x - 1, y - 1);
        bool diagTR = IsLandDiag(map, rumTile, x + 1, y - 1);
        bool diagBR = IsLandDiag(map, rumTile, x + 1, y + 1);
        bool diagBL = IsLandDiag(map, rumTile, x - 1, y + 1);

        if (top && right && left && bottom)
        {
            if (!diagTL) return 10;
            if (!diagTR) return 13;
            if (!diagBR) return 11;
            if (!diagBL) return 12;

            return 5;
        }

        bool hasWaterNeighbor = !top || !right || !left || !bottom;

        if (!hasWaterNeighbor)
            return 5;

        bool wTop = !top;
        bool wRight = !right;
        bool wLeft = !left;
        bool wBottom = !bottom;

        if (wTop && !wRight && !wLeft && !wBottom) return 8;
        if (!wTop && wRight && !wLeft && !wBottom) return 6;
        if (!wTop && !wRight && !wLeft && wBottom) return 2;
        if (!wTop && !wRight && wLeft && !wBottom) return 4;

        if (wTop && wLeft) return 7;
        if (wTop && wRight) return 9;
        if (wBottom && wLeft) return 1;
        if (wBottom && wRight) return 3;

        return 5;
    }
}