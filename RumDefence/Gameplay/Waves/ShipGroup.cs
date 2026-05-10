using System.Collections.Generic;

namespace RumDefence;

public record TroopGroup(TroopData Data, int Count);

public class ShipGroup
{
    public Ship.Data Data { get; }
    public int Count { get; set; }
    public IReadOnlyList<TroopGroup> Troops { get; }
    public int TotalTroops { get; }
    public float TroopSpawnDelay { get; }

    public ShipGroup(Ship.Data data, int count, IReadOnlyList<TroopGroup> troops, float troopSpawnDelay)
    {
        Data = data;
        Count = count;
        Troops = troops;
        TroopSpawnDelay = troopSpawnDelay;

        int total = 0;
        foreach (var t in troops) total += t.Count;
        TotalTroops = total;
    }
}
