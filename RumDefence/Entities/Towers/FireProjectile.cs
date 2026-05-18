using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class FireProjectile : BaseProjectile
{
    private readonly float _burnDuration;
    private readonly float _burnDamagePerTick;
    private readonly float _burnTickInterval;

    public FireProjectile(Vector2 start, Troop target, float speed, int damage,
        float burnDuration = 3f, float burnDamagePerTick = 4f, float burnTickInterval = 0.5f)
        : base(start, target, speed, damage)
    {
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
            if (!Target.IsDead && !Target.IsFinished)
                Target.ApplyModifier(new BurnModifier(_burnDuration, _burnDamagePerTick, _burnTickInterval));

            GameScreen.Instance.FireEffects.Add(new FireEffect(Position, 30f));
        }
    }
}
