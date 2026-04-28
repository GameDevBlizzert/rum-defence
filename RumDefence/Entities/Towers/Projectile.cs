using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence;

namespace RumDefence;

public class Projectile : Entity
{
    private readonly Troop _target;
    private readonly float _speed;
    private readonly int _damage;
    private Vector2 _lastKnownPosition;

    public bool IsFinished { get; private set; }

    private static Texture2D _cannonBall;

    public Projectile(Vector2 start, Troop target, float speed, int damage)
    {
        _target = target;
        _speed = speed;
        _damage = damage;
        Position = start;
        _lastKnownPosition = target.Position;

        if (_cannonBall == null)
        {
            _cannonBall = RumGame.Instance.Content.Load<Texture2D>("KenneyPiratePack/PNG/Retina/Ship parts/cannonBall");
        }

        Texture = _cannonBall;
        origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
        Size = SizeSystem.Square(0.25f);
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
