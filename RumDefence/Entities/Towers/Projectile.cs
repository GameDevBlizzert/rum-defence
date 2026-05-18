using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence;

namespace RumDefence;

public abstract class BaseProjectile : Entity
{
    private readonly float _speed;
    private Vector2 _lastKnownPosition;

    public Troop Target { get; }
    public float Damage { get; }
    public bool IsFinished { get; set; }

    protected bool ApplyDirectDamage = true;

    protected static Texture2D _texture;

    public BaseProjectile(Vector2 start, Troop target, float speed, int damage)
    {
        Target = target;
        _speed = speed;
        Damage = damage;
        Position = start;
        _lastKnownPosition = target.Position;
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
            if (ApplyDirectDamage && !Target.IsDead && !Target.IsFinished)
                Target.TakeDamage(Damage);

            IsFinished = true;
            return;
        }

        dir.Normalize();
        Position += dir * _speed * dt;
    }
}
