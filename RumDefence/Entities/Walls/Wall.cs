using Microsoft.Xna.Framework;

namespace RumDefence;

public class Wall
{
    public Point GridPos;

    public bool IsDamaged;

    public Wall(Point gridPos)
    {
        GridPos = gridPos;
    }
}