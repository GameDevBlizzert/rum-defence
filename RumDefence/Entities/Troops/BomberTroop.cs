using Microsoft.Xna.Framework;

namespace RumDefence;

public class BomberTroop : Troop
{
    public BomberTroop(TroopData data, Vector2 start, Vector2 target)
        : base(data, start, target)
    {
        animation = new BomberTroopAnimation(16, 16, 0.2f, 3, true);
        _swordAttackAnimation = new BomberTroopExplodeAnimation();
        _dyingAnimation = new BomberTroopDyingAnimation();
    }
}