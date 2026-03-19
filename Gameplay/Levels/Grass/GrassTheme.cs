using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class GrassTheme : ITileTheme
{
    private Dictionary<int, List<Texture2D>> tileMap;

    private string envPrefix = "Art/Themes/Grass/Environment/";
    private string shipPrefix = "Art/Themes/Grass/Ships/";
    private string enemyPrefix = "Art/Themes/Grass/Enemies/";

    private Dictionary<string, Texture2D> shipMap;
    private List<Texture2D> enemies;

    public GrassTheme()
    {
        tileMap = new Dictionary<int, List<Texture2D>>()
        {
            { 0 , new List<Texture2D>() },

            { 1, LoadEnv("1") },
            { 2, LoadEnv("2.1", "2.2") },
            { 3, LoadEnv("3") },
            { 4, LoadEnv("4.1", "4.2") },
            { 5, LoadEnv("5.1", "5.2", "5.3", "5.4") },
            { 6, LoadEnv("6") },
            { 7, LoadEnv("7") },
            { 8, LoadEnv("8.1", "8.2") },
            { 9, LoadEnv("9") }
        };

        shipMap = new Dictionary<string, Texture2D>()
        {
            { "ship_1", LoadShip("ship_1")  }
        };

        enemies = new List<Texture2D>()
        {
            LoadEnemy("enemy_1")
        };
    }

    // =====================
    // LOAD HELPERS
    // =====================

    private List<Texture2D> LoadEnv(params string[] names)
    {
        var list = new List<Texture2D>();

        foreach (var name in names)
            list.Add(RumGame.Instance.Content.Load<Texture2D>(envPrefix + name));

        return list;
    }

    private Texture2D LoadShip(string name)
    {
        return RumGame.Instance.Content.Load<Texture2D>(shipPrefix + name);
    }

    private Texture2D LoadEnemy(string name)
    {
        return RumGame.Instance.Content.Load<Texture2D>(enemyPrefix + name);
    }

    // =====================
    // TILE
    // =====================

    public Texture2D GetTexture(int c, int x, int y)
    {
        if (!tileMap.TryGetValue(c, out var list) || list.Count == 0)
            return null;

        if (list.Count == 1)
            return list[0];

        int seed = x * 73856093 ^ y * 19349663;
        var rng = new Random(seed);

        return list[rng.Next(list.Count)];
    }

    // =====================
    // SHIPS
    // =====================

    public Texture2D GetShip(string name)
    {
        if (shipMap.TryGetValue(name, out var tex))
            return tex;

        return shipMap["ship_1"]; // fallback
    }

    // =====================
    // ENEMIES
    // =====================

    public Texture2D GetRandomEnemy()
    {
        var rng = new Random();
        return enemies[rng.Next(enemies.Count)];
    }
}