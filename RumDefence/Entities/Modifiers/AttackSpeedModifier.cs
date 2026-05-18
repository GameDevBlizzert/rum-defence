using Microsoft.Xna.Framework;
namespace RumDefence;

public class AttackSpeedModifier : IModifier
{
    private float multiplier;
    private float _duration;
    public bool IsExpired => _duration <= 0f;
    public AttackSpeedModifier(float duration, float multiplier = 0.3f)
    {
        this.multiplier = multiplier;
        _duration = duration;
    }
    public void Apply(Troop troop)
    {
        troop.AttackSpeedMultiplier *= multiplier;
    }
    public void Refresh(IModifier source)
    {
        if (source is AttackSpeedModifier s)
        {
            _duration = s._duration;
            multiplier = s.multiplier;
        }
    }
    public void Update(Troop troop, GameTime gameTime)
    {
        _duration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (!IsExpired)
            Apply(troop);
    }
}
