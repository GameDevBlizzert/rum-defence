using Microsoft.Xna.Framework;
using RumDefence;
using System.Collections.Generic;

namespace RumDefence;

public class MusketTower : BaseTower
{
    public MusketTower(Vector2 location, List<Troop> troops) : base(location, troops, "Art/Towers/musket")
    {
        Range = 500f;
        FireRate = 3f;
        Damage = 15;
        ProjectileSpeed = 500f;
        AttackMode = AttackMode.First;
    }
}
