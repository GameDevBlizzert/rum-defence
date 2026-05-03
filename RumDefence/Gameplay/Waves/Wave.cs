using System.Collections.Generic;

namespace RumDefence;

public class Wave
{
    public List<ShipGroup> ShipGroups { get; }
    public float WaveDuration { get; }

    public Wave(List<ShipGroup> groups, float waveDuration)
    {
        ShipGroups = groups;
        WaveDuration = waveDuration;
    }
}
