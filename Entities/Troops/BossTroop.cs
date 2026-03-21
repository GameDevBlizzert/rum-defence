using Microsoft.Xna.Framework;

namespace RumDefence;

public class BossTroop : Troop
{
    public BossTroop(Vector2 start, Vector2 target)
        : base(start, target)
    {
        Health = 300;

        SpeedMultiplier = 0.5f;
    }
}