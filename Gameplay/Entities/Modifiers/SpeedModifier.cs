namespace RumDefence;

public class SpeedModifier : IModifier
{
    private float multiplier;

    public SpeedModifier(float multiplier)
    {
        this.multiplier = multiplier;
    }

    public void Apply(Enemy enemy)
    {
        enemy.Speed *= multiplier;
    }
}