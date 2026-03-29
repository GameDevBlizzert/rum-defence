using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence;

public class GrassTheme : BaseTheme
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

            { 1, LoadList(envPrefix, "1") },
            { 2, LoadList(envPrefix, "2.1", "2.2") },
            { 3, LoadList(envPrefix, "3") },
            { 4, LoadList(envPrefix, "4.1", "4.2") },
            { 5, LoadList(envPrefix, "5.1", "5.2", "5.3", "5.4") },
            { 6, LoadList(envPrefix, "6") },
            { 7, LoadList(envPrefix, "7") },
            { 8, LoadList(envPrefix, "8.1", "8.2") },
            { 9, LoadList(envPrefix, "9") }
        };

        shipMap = new Dictionary<string, Texture2D>()
        {
            { "ship_1", Load(shipPrefix + "ship_1") },
            { "boss_ship", Load(shipPrefix + "boss_ship") }
        };

        enemies = new List<Texture2D>()
        {
            Load(enemyPrefix + "enemy_1")
        };
    }

    public override Texture2D GetTexture(int c, int x, int y)
    {
        if (!tileMap.TryGetValue(c, out var list) || list.Count == 0)
            return null;

        return GetSeeded(list, x, y);
    }

    public override Texture2D GetShip(string name)
    {
        if (shipMap.TryGetValue(name, out var tex))
            return tex;

        return shipMap["ship_1"];
    }

    public override Texture2D GetRandomEnemy()
    {
        return GetRandom(enemies);
    }
}