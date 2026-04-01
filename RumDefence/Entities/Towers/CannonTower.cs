using Microsoft.Xna.Framework;
using RumDefence;
using System.Collections.Generic;

namespace Rum_Defence.Entities.Towers
{
    public class CannonTower : BaseTower
    {
        public CannonTower(Vector2 location, List<Troop> troops) : base(location, troops)
        {
            Range = 700f;
            FireRate = 1.5f;
            Damage = 40;
            ProjectileSpeed = 300f;
            AttackMode = AttackMode.Closest;
        }
    }
}
