namespace RumDefence;

public enum AttackMode
{
    Nearest,
    Strongest,
    Farthest,
    ClosestToRum
}

public static class AttackModeExtensions
{
    public static string ToDisplayName(this AttackMode mode)
    {
        return mode switch
        {
            AttackMode.Nearest => "Nearest",
            AttackMode.Strongest => "Strongest",
            AttackMode.Farthest => "Farthest",
            AttackMode.ClosestToRum => "Closest to Rum",
            _ => mode.ToString()
        };
    }
}
