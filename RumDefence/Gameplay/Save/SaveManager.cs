using System;
using System.IO;
using System.Text.Json;

namespace RumDefence;

public static class SaveManager
{
    private static readonly string SavePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RumDefence",
        "save.json"
    );

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static SaveData CurrentSave { get; private set; } = new();

    public static void Load()
    {
        try
        {
            if (File.Exists(SavePath))
                CurrentSave = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(SavePath)) ?? new SaveData();
        }
        catch
        {
            CurrentSave = new SaveData();
        }
    }

    public static void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath)!);
            File.WriteAllText(SavePath, JsonSerializer.Serialize(CurrentSave, JsonOptions));
        }
        catch { }
    }

    public static void UnlockLevel(Level level)
    {
        if (level == null || string.IsNullOrEmpty(level.SaveKey))
            return;

        level.IsUnlocked = true;
        CurrentSave.UnlockedLevelKeys.Add(level.SaveKey);
        Save();
    }

    public static void SaveLevelScore(Level level, int coins, int waves)
    {
        if (level == null || string.IsNullOrEmpty(level.SaveKey))
            return;

        if (!CurrentSave.LevelScores.TryGetValue(level.SaveKey, out var score))
        {
            score = new LevelScoreData();
            CurrentSave.LevelScores[level.SaveKey] = score;
        }

        if (coins > score.BestCoins)
            score.BestCoins = coins;

        if (waves > score.BestWaves)
            score.BestWaves = waves;

        Save();
    }

    public static LevelScoreData GetLevelScore(Level level)
    {
        if (level == null || string.IsNullOrEmpty(level.SaveKey))
            return null;

        CurrentSave.LevelScores.TryGetValue(level.SaveKey, out var score);
        return score;
    }
}