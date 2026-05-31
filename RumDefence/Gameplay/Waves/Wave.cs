using System.Collections.Generic;

namespace RumDefence;

public class Wave
{
    public List<ShipGroup> ShipGroups { get; }
    public float MinSpawnTime { get; }
    public float MaxSpawnTime { get; }

    public Wave(List<ShipGroup> groups, float minSpawnTime, float maxSpawnTime)
    {
        ShipGroups = groups;
        MinSpawnTime = minSpawnTime;
        MaxSpawnTime = maxSpawnTime;
    }
}
