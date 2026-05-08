using System.Collections.Generic;

namespace RumDefence;

public class Wave
{
    public List<ShipGroup> ShipGroups { get; }
    public float MinSpawnTime { get; }
    public float MaxSpawnTime { get; }
    public float HoldingTime { get; }

    public Wave(List<ShipGroup> groups, float minSpawnTime, float maxSpawnTime, float holdingTime = 0f)
    {
        ShipGroups = groups;
        MinSpawnTime = minSpawnTime;
        MaxSpawnTime = maxSpawnTime;
        HoldingTime = holdingTime;
    }
}
