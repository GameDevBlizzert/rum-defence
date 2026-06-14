using Microsoft.Xna.Framework;

namespace RumDefence;

// Doet % damage op basis van de health per seconde. handig voor hoge HP troepen.
public class PoisonModifier : IModifier
{
    private float _duration;
    private float _damagePercentPerTick;
    private float _tickInterval;
    private float _tickTimer;

    public bool IsExpired => _duration <= 0f;

    public PoisonModifier(float duration, float damagePercentPerTick, float tickInterval)
    {
        _duration = duration;
        _damagePercentPerTick = damagePercentPerTick;
        _tickInterval = tickInterval;
        _tickTimer = tickInterval;
    }

    public void Apply(Troop troop) { }

    public void Refresh(IModifier source)
    {
        if (source is PoisonModifier p)
        {
            _duration = p._duration;
            _damagePercentPerTick = p._damagePercentPerTick;
            _tickInterval = p._tickInterval;
        }
    }

    public void Update(Troop troop, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _duration -= dt;
        _tickTimer -= dt;

        if (_tickTimer <= 0f && !IsExpired)
        {
            troop.TakeDamage(troop.Health.InitialHealth * _damagePercentPerTick);
            _tickTimer += _tickInterval;
        }
    }
}
