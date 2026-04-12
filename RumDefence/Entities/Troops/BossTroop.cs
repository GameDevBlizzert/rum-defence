using Microsoft.Xna.Framework;

namespace RumDefence;

public class BossTroop : Troop
{
    public BossTroop(string spritePath, Vector2 start, Vector2 target)
        : base(spritePath, start, target)
    {
        animation = new BossTroopAnimation(
            16,
            16,
            0.2f,
            3,
            true
        );
        Health = 300;
        SpeedMultiplier = 0.5f;
        Size = SizeSystem.Square(12f);
        ApplySize();
    }
}