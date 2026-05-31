using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class KnifeProjectile : BaseProjectile
{
    private const int FrameWidth = 32;
    private const int FrameHeight = 32;
    private const float RotationSpeed = 7f;
    public KnifeProjectile(Vector2 start, Troop target, float speed, int damage)
        : base(start, target, speed, damage)
    {
        Texture = RumGame.Instance.Content.Load<Texture2D>("Art/Projectiles/bandit-knife");
        origin = new Vector2(FrameWidth / 2f, FrameHeight / 2f);
        Size = SizeSystem.Square(0.5f);
        scale = SizeSystem.ToScale(FrameWidth, Size);
    }

    // 2% of max HP per tick over 4 seconds — scales naturally against high-HP units.
    private const float PoisonDuration = 4f;
    private const float PoisonDamagePercentPerTick = 0.02f;
    private const float PoisonTickInterval = 0.5f;

    public override void Update(GameTime gameTime)
    {
        if (IsFinished) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        rotation += RotationSpeed * dt;

        base.Update(gameTime);

        if (IsFinished && !Target.IsDead)
            Target.ApplyModifier(new PoisonModifier(PoisonDuration, PoisonDamagePercentPerTick, PoisonTickInterval));
    }
}
