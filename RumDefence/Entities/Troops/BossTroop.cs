using Microsoft.Xna.Framework;

namespace RumDefence;

public class BossTroop : Troop
{
    public BossTroop(TroopData data, Vector2 start, Vector2 target)
        : base(data, start, target)
    {
        animation = new BossTroopAnimation(16, 16, 0.2f, 3, true);
        _swordAttackAnimation = new BossTroopSwordAttackAnimation();
    }
}