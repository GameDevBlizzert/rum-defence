using System.Collections.Generic;

namespace RumDefence;

public class SaveData
{
    public HashSet<string> UnlockedLevelKeys { get; set; } = new();
    public Dictionary<string, LevelScoreData> LevelScores { get; set; } = new();
    public HashSet<string> UnlockedTowers { get; set; } = new();
    public HashSet<string> EncounteredTroops { get; set; } = new();
    public HashSet<string> FreeTowerPlacementsClaimed { get; set; } = new();
    public float MusicVolume { get; set; } = 0.5f;
    public float SfxVolume { get; set; } = 1.0f;
    public Dictionary<string, string> KeyBindings { get; set; } = new();
}

public class LevelScoreData
{
    public int BestCoins { get; set; }
    public int BestWaves { get; set; }
}