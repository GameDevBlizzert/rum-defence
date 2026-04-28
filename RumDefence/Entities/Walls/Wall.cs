using Microsoft.Xna.Framework;

namespace RumDefence;

public class Wall
{
    public Point GridPos;

    public bool IsDamaged;
    public bool IsDiagonal;

    public Wall(Point gridPos, bool isDiagonal = false)
    {
        GridPos = gridPos;
        // IsDiagonal = isDiagonal;
    }
}