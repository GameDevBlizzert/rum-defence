using Microsoft.Xna.Framework;

namespace RumDefence;

public class GruntTroop : Troop
{
    public GruntTroop(TroopData data, Vector2 start, Vector2 target)
        : base(data, start, target)
    {
    }
}