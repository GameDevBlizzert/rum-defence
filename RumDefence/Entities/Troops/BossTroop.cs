using Microsoft.Xna.Framework;

namespace RumDefence;

public class BossTroop : Troop
{
    public BossTroop(TroopData data, Vector2 start, Vector2 target)
        : base(data, start, target)
    {
        // _walkanimation = new BossTroopAnimation(16, 16, 0.2f, true);
        // _swordAttackAnimation = new BossTroopSwordAttackAnimation();
    }
}