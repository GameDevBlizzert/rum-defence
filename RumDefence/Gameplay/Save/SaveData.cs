using System.Collections.Generic;

namespace RumDefence;

public class SaveData
{
    public HashSet<string> UnlockedLevelKeys { get; set; } = new();
    public float MusicVolume { get; set; } = 0.5f;
    public float SfxVolume { get; set; } = 1.0f;
}