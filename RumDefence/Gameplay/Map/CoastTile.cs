using Microsoft.Xna.Framework;

namespace RumDefence;

public class CoastTile
{
    public Point GridPos;
    public int TileType;

    public CoastTile(Point pos, int type)
    {
        GridPos = pos;
        TileType = type;
    }
}