using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence;

public class GrassTheme : BaseTheme, IWallTheme
{
    private string envPrefix = "Art/Themes/Grass/Environment/";
    private string wallPrefix = "Art/Themes/Grass/Walls/";
    private string shipPrefix = "Art/Themes/Grass/Ships/";
    private string enemyPrefix = "Art/Themes/Grass/Enemies/";

    private Dictionary<int, List<Texture2D>> tileMap;

    public Texture2D Single { get; private set; }
    public Texture2D Wall { get; private set; }
    public Texture2D End { get; private set; }
    public Texture2D Corner { get; private set; }
    public Texture2D Twall { get; private set; }
    public Texture2D Xwall { get; private set; }
    public Texture2D Diagonal { get; private set; }
    public Texture2D DiagonalEnd { get; private set; }
    public Texture2D DiagonalFill { get; private set; }

    private List<Texture2D> wallDamagedList;
    private List<Texture2D> endDamagedList;
    private List<Texture2D> cornerDamagedList;
    private List<Texture2D> twallDamagedList;
    private List<Texture2D> xwallDamagedList;

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

        Single = Load(wallPrefix + "single");
        Wall = Load(wallPrefix + "wall");
        End = Load(wallPrefix + "end");
        Corner = Load(wallPrefix + "corner");
        Twall = Load(wallPrefix + "Twall");
        Xwall = Load(wallPrefix + "Xwall");
        Diagonal = Load(wallPrefix + "diagonal");
        DiagonalEnd = Load(wallPrefix + "diagonal_end");
        DiagonalFill = Load(wallPrefix + "diagonal_fill");

        wallDamagedList = LoadList(wallPrefix, "wall_damaged_1", "wall_damaged_2");
        endDamagedList = LoadList(wallPrefix, "end_damaged");
        cornerDamagedList = LoadList(wallPrefix, "corner_damaged");
        twallDamagedList = LoadList(wallPrefix, "Twall_damaged");
        xwallDamagedList = LoadList(wallPrefix, "Xwall_damaged");

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

    public Texture2D GetDamagedWall(int x, int y)
    {
        return GetSeeded(wallDamagedList, x, y);
    }

    public Texture2D GetDamagedEnd(int x, int y)
    {
        return GetSeeded(endDamagedList, x, y);
    }

    public Texture2D GetDamagedCorner(int x, int y)
    {
        return GetSeeded(cornerDamagedList, x, y);
    }

    public Texture2D GetDamagedTwall(int x, int y)
    {
        return GetSeeded(twallDamagedList, x, y);
    }

    public Texture2D GetDamagedXwall(int x, int y)
    {
        return GetSeeded(xwallDamagedList, x, y);
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