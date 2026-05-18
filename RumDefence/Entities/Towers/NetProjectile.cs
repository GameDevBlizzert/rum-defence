using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class NetProjectile : BaseProjectile
{
    private readonly float _aoeRadius;
    private readonly float _slowMultiplier;
    private readonly float _debuffDuration;

    public NetProjectile(Vector2 start, Troop target, float speed, float aoeRadius)
        : base(start, target, speed, 0)
    {
        ApplyDirectDamage = false;
        _aoeRadius = aoeRadius;
        _slowMultiplier = 0.5f;
        _debuffDuration = 1f;

        Texture = RumGame.Instance.Content.Load<Texture2D>("Art/Towers/fishing-net");

        origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
        float diameter = aoeRadius * 2f;
        Size = new Vector2(diameter, diameter);
        ApplySize();
    }

    public override void Update(GameTime gameTime)
    {
        if (IsFinished) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        rotation += 6f * dt;

        base.Update(gameTime);

        if (IsFinished)
            GameScreen.Instance.NetEffects.Add(new NetEffect(Position, _debuffDuration, _aoeRadius, _slowMultiplier));
    }
}
