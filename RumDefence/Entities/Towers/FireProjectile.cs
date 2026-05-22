using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class FireProjectile : BaseProjectile
{
    private readonly float _aoeRadius;
    private readonly float _burnDuration;
    private readonly float _burnDamagePerTick;
    private readonly float _burnTickInterval;

    public FireProjectile(Vector2 start, Troop target, float speed, int damage, float aoeRadius,
        float burnDuration = 3f, float burnDamagePerTick = 4f, float burnTickInterval = 0.5f)
        : base(start, target, speed, damage)
    {
        ApplyDirectDamage = false;
        _aoeRadius = aoeRadius;
        _burnDuration = burnDuration;
        _burnDamagePerTick = burnDamagePerTick;
        _burnTickInterval = burnTickInterval;

        Texture = RumGame.Instance.Content.Load<Texture2D>("KenneyPiratePack/PNG/Retina/Ship parts/cannonBall");
        origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
        Size = SizeSystem.Square(0.3f);
        ApplySize();
        color = Color.OrangeRed;
    }

    public override void Update(GameTime gameTime)
    {
        if (IsFinished) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        rotation += 8f * dt;

        base.Update(gameTime);

        if (IsFinished)
        {
            GameScreen.Instance.FireEffects.Add(new FireEffect(Position, _aoeRadius));

            foreach (var troop in GameScreen.Instance.Troops)
            {
                if (troop.IsDead || troop.IsFinished) continue;

                float dist = Vector2.Distance(Position, troop.Position);
                if (dist > _aoeRadius) continue;

                float distFraction = dist / _aoeRadius;

                // Splash damage: 80% at center, 20% at edge
                float dmgFraction = 0.8f - (distFraction * 0.6f);
                troop.TakeDamage(Damage * dmgFraction);

                // Burn duration: full at center, 30% at edge
                float durationFraction = 1f - (distFraction * 0.7f);
                troop.ApplyModifier(new BurnModifier(
                    _burnDuration * durationFraction,
                    _burnDamagePerTick,
                    _burnTickInterval));
            }
        }
    }
}
