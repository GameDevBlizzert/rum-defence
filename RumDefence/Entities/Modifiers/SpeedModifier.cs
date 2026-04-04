namespace RumDefence;

public class SpeedModifier : IModifier
{
    private float multiplier;

    public SpeedModifier(float multiplier)
    {
        this.multiplier = multiplier;
    }

    public void Apply(Troop troop)
    {
        troop.SpeedMultiplier *= multiplier;
    }
}