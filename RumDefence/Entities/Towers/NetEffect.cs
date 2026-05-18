using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class NetEffect : Entity
{
    private float _lifeTime;
    private readonly float _maxLifeTime;
    private readonly float _aoeRadius;
    private readonly float _slowMultiplier;
    private readonly float _attackSlowMultiplier;

    public bool IsFinished => _lifeTime <= 0f;

    private static Texture2D _netTexture;

    public NetEffect(Vector2 position, float duration, float radius, float slowMultiplier, float attackSlowMultiplier = 0.3f)
    {
        _lifeTime = duration;
        _maxLifeTime = duration;
        _aoeRadius = radius;
        _slowMultiplier = slowMultiplier;
        _attackSlowMultiplier = attackSlowMultiplier;
        Position = position;

        if (_netTexture == null)
            _netTexture = RumGame.Instance.Content.Load<Texture2D>("Art/Towers/fishing-net");

        Texture = _netTexture;
        origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);

        float diameter = radius * 2f;
        Size = new Vector2(diameter, diameter);
        ApplySize();
    }

    public override void Update(GameTime gameTime)
    {
        _lifeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        color = Color.White * (_lifeTime / _maxLifeTime);

        foreach (var troop in GameScreen.Instance.Troops)
        {
            if (troop.IsDead || troop.IsFinished) continue;
            if (Vector2.Distance(Position, troop.Position) <= _aoeRadius)
            {
                troop.ApplyModifier(new SpeedModifier(_lifeTime, _slowMultiplier));
                troop.ApplyModifier(new AttackSpeedModifier(_lifeTime, _attackSlowMultiplier));
            }
        }
    }
}
