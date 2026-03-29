namespace RumDefence;

public class ShipGroup
{
    public Ship.Data Data { get; }
    public int Count { get; set; }

    public ShipGroup(Ship.Data data, int count)
    {
        Data = data;
        Count = count;
    }
}