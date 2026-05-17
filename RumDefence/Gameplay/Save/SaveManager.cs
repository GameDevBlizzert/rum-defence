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
}