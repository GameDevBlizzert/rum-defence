using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence;

namespace Rum_Defence.Entities.Towers
{
    public class Projectile : Entity
    {
        private readonly Troop _target;
        private readonly float _speed;
        private readonly int _damage;
        private Vector2 _lastKnownPosition;

        public bool IsFinished { get; private set; }

        private static Texture2D _pixel;

        public Projectile(Vector2 start, Troop target, float speed, int damage)
        {
            _target = target;
            _speed = speed;
            _damage = damage;
            Position = start;
            _lastKnownPosition = target.Position;

            if (_pixel == null)
            {
                _pixel = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.Orange });
            }

            Texture = _pixel;
            origin = Vector2.Zero;
            Size = SizeSystem.Square(0.12f);
            ApplySize();
        }

        public override void Update(GameTime gameTime)
        {
            if (IsFinished) return;

            if (!(_target.IsDead || _target.IsFinished))
                _lastKnownPosition = _target.Position;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 dir = _lastKnownPosition - Position;

            if (dir.Length() < 6f)
            {
                if (!_target.IsDead && !_target.IsFinished)
                    _target.TakeDamage(_damage);

                IsFinished = true;
                return;
            }

            dir.Normalize();
            Position += dir * _speed * dt;
        }
    }
}
