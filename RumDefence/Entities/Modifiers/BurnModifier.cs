using Microsoft.Xna.Framework;
namespace RumDefence;

public class BurnModifier : IModifier
{
    private float _duration;
    private float _damagePerTick;
    private float _tickInterval;
    private float _tickTimer;

    public bool IsExpired => _duration <= 0f;

    public BurnModifier(float duration, float damagePerTick, float tickInterval)
    {
        _duration = duration;
        _damagePerTick = damagePerTick;
        _tickInterval = tickInterval;
        _tickTimer = tickInterval;
    }

    public void Apply(Troop troop) { }

    public void Refresh(IModifier source)
    {
        if (source is BurnModifier b)
        {
            _duration = b._duration;
            _damagePerTick = b._damagePerTick;
            _tickInterval = b._tickInterval;
        }
    }

    public void Update(Troop troop, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _duration -= dt;
        _tickTimer -= dt;

        if (_tickTimer <= 0f && !IsExpired)
        {
            troop.TakeDamage(_damagePerTick);
            _tickTimer += _tickInterval;
        }
    }
}
