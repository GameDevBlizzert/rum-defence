using System;
using System.Collections.Generic;
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

    public static void ApplySaveDataToLevels(List<Level> levels)
    {
        if (levels == null)
            return;

        foreach (var level in levels)
        {
            if (string.IsNullOrEmpty(level.SaveKey))
                continue;

            if (CurrentSave.UnlockedLevelKeys.Contains(level.SaveKey))
                level.IsUnlocked = true;
        }
    }

    public static void UnlockLevel(Level level)
    {
        if (level == null || string.IsNullOrEmpty(level.SaveKey))
            return;

        level.IsUnlocked = true;
        CurrentSave.UnlockedLevelKeys.Add(level.SaveKey);
        Save();
    }

    public static void UnlockNextLevel(List<Level> levelSet, Level currentLevel)
    {
        if (levelSet == null || currentLevel == null)
            return;

        int index = levelSet.FindIndex(l => l.Id == currentLevel.Id);

        if (index < 0 || index >= levelSet.Count - 1)
            return;

        UnlockLevel(levelSet[index + 1]);
    }

    public static void SaveWinScore(Level level, int coins, int waves)
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

    public static bool IsTowerUnlocked(TowerType type)
        => CurrentSave.UnlockedTowers.Contains(type.ToString());

    public static void MarkTowerUnlocked(TowerType type)
    {
        if (CurrentSave.UnlockedTowers.Add(type.ToString()))
            Save();
    }

    public static bool IsTroopEncountered(TroopType type)
        => CurrentSave.EncounteredTroops.Contains(type.ToString());

    public static void MarkTroopEncountered(TroopType type)
    {
        if (CurrentSave.EncounteredTroops.Add(type.ToString()))
            Save();
    }

    public static bool HasClaimedFreeTowerPlacement(TowerType type)
        => CurrentSave.FreeTowerPlacementsClaimed.Contains(type.ToString());

    public static void MarkFreeTowerPlacementClaimed(TowerType type)
    {
        if (CurrentSave.FreeTowerPlacementsClaimed.Add(type.ToString()))
            Save();
    }

    public static bool HasCompletedAllGrassLevels()
    {
        for (int i = 1; i <= 4; i++)
            if (!CurrentSave.LevelScores.ContainsKey($"grass_{i}"))
                return false;

        return true;
    }
}