using Microsoft.Xna.Framework;

namespace RumDefence;

public class NetProjectile : Projectile
{
    private readonly float _aoeRadius;
    private readonly float _slowMultiplier;
    private readonly float _debuffDuration;

    public NetProjectile(Vector2 start, Troop target, float speed, float aoeRadius, float slowMultiplier = 0.3f, float debuffDuration = 3f)
        : base(start, target, speed, 0)
    {
        ApplyDirectDamage = false;
        _aoeRadius = aoeRadius;
        _slowMultiplier = slowMultiplier;
        _debuffDuration = debuffDuration;
        color = Color.Cyan;
    }

    public override void Update(GameTime gameTime)
    {
        if (IsFinished) return;

        base.Update(gameTime);

        if (IsFinished)
        {
            GameScreen.Instance.NetEffects.Add(new NetEffect(Position, _debuffDuration, _aoeRadius));

            foreach (var troop in GameScreen.Instance.Troops)
            {
                if (troop.IsDead || troop.IsFinished) continue;
                if (Vector2.Distance(Position, troop.Position) <= _aoeRadius)
                    troop.ApplyModifier(new SpeedModifier(_debuffDuration, _slowMultiplier));
            }
        }
    }
}
