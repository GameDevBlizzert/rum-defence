using Microsoft.Xna.Framework;
namespace RumDefence;

public class SpeedModifier : IModifier
{
    private float multiplier;
    private float _duration;
    public bool IsExpired => _duration <= 0f;
    public SpeedModifier(float duration, float multiplier = 0.3f)
    {
        this.multiplier = multiplier;
        _duration = duration;
    }
    public void Apply(Troop troop)
    {
        troop.SpeedMultiplier *= multiplier;
    }
    public void Refresh(float duration) => _duration = duration;
    public void Update(Troop troop, GameTime gameTime)
    {
        _duration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (!IsExpired)
            Apply(troop);
    }
}