using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence;

namespace RumDefence;

public class Projectile : Entity
{
    private readonly float _speed;
    private Vector2 _lastKnownPosition;

    public Troop Target { get; }
    public int Damage { get; }
    public bool IsFinished { get; private set; }

    private static Texture2D _cannonBall;

    public Projectile(Vector2 start, Troop target, float speed, int damage)
    {
        Target = target;
        _speed = speed;
        Damage = damage;
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

        if (!(Target.IsDead || Target.IsFinished))
            _lastKnownPosition = Target.Position;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 dir = _lastKnownPosition - Position;

        if (dir.Length() < 6f)
        {
            if (!Target.IsDead && !Target.IsFinished)
                Target.TakeDamage(Damage);

            IsFinished = true;
            return;
        }

        dir.Normalize();
        Position += dir * _speed * dt;
    }
}
