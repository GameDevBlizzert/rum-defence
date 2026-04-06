namespace RumDefence;

public enum AttackMode
{
    Closest,   // Nearest troop to the tower
    Strongest, // Troop with the highest current health
    First      // Troop furthest along the path (closest to the goal)
}
