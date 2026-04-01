using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence;
using System.Collections.Generic;

namespace Rum_Defence.Entities.Towers
{
    public class BaseTower : Entity
    {
        private static Texture2D _pixel;

        protected readonly List<Troop> Troops;
        private readonly List<Projectile> _projectiles = [];

        public float Range { get; set; } = 700f;
        public float FireRate { get; set; } = 1f; // shots per second
        public int Damage { get; set; } = 25;
        public float ProjectileSpeed { get; set; } = 200f;
        public AttackMode AttackMode { get; set; } = AttackMode.Closest;

        private float _fireCooldown = 0f;

        public BaseTower(Vector2 location, List<Troop> troops)
        {
            Position = location;
            Troops = troops;

            _pixel = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.Red });

            Texture = _pixel;
            origin = Vector2.Zero;

            Size = SizeSystem.Square(0.3f);
            ApplySize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //update projectiles
            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                _projectiles[i].Update(gameTime);
                if (_projectiles[i].IsFinished)
                    _projectiles.RemoveAt(i);
            }

            // handle firing
            _fireCooldown -= dt;
            if (_fireCooldown > 0f) return;

            Troop target = FindTarget();
            if (target == null) return;

            _projectiles.Add(new Projectile(Position, target, ProjectileSpeed, Damage));
            _fireCooldown = 1f / FireRate;
        }

        private Troop FindTarget()
        {
            Troop best = null;
            float bestValue = float.MaxValue;

            foreach (var troop in Troops)
            {
                if (troop.IsDead || troop.IsFinished) continue;

                float dist = Vector2.Distance(Position, troop.Position);
                if (dist > Range) continue;

                float value = AttackMode switch
                {
                    AttackMode.Closest => dist,
                    AttackMode.Strongest => -troop.Health, // highest HP = lowest value
                    AttackMode.First => dist,          // TODO: replace with path progress location of the rum
                    _ => dist
                };

                if (value < bestValue)
                {
                    bestValue = value;
                    best = troop;
                }
            }

            return best;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (var proj in _projectiles)
                proj.Draw(spriteBatch);
        }
    }
}
