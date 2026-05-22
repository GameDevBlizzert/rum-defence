using Microsoft.Xna.Framework.Graphics;
using Moq;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class GhostTheme : BaseTheme, IWallTheme
{
    private string envPrefix = "Art/Themes/Ghost/Environment/";//ghost map
    private string wallPrefix = "Art/Themes/Grass/Walls/";
    private string shipPrefix = "Art/Themes/Ghost/Ships/";//ghost ship
    private string enemyPrefix = "Art/Themes/Grass/Enemies/";
    private string decorationPrefix = "Art/Themes/Grass/Decorations/";

    private Dictionary<int, List<Texture2D>> tileMap;
    private Dictionary<string, Texture2D> shipMap;
    private List<Texture2D> enemies;

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
    private List<Texture2D> rocks;

    public GhostTheme()
    {
        tileMap = new Dictionary<int, List<Texture2D>>()
        {
            { 0 , new List<Texture2D>() },

            { 1, LoadList(envPrefix, "1") },
            { 2, LoadList(envPrefix, "2") },
            { 3, LoadList(envPrefix, "3") },
            { 4, LoadList(envPrefix, "4") },
            { 5, LoadList(envPrefix, "5") },
            { 6, LoadList(envPrefix, "6") },
            { 7, LoadList(envPrefix, "7") },
            { 8, LoadList(envPrefix, "8") },
            { 9, LoadList(envPrefix, "9") },
            { 10, LoadList(envPrefix, "10") },
            { 11, LoadList(envPrefix, "11") },
            { 12, LoadList(envPrefix, "12") },
            { 13, LoadList(envPrefix, "13") }
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

        rocks = new List<Texture2D>()
        {
            Load(decorationPrefix + "Rock_01"),
            Load(decorationPrefix + "Rock_02"),
            Load(decorationPrefix + "Rock_03"),
            Load(decorationPrefix + "Rock_Moss_01"),
            Load(decorationPrefix + "Rock_Moss_02"),
            Load(decorationPrefix + "Rock_Moss_03"),
        };

    }

    public override Texture2D GetTexture(int tile, int x, int y)
    {
        if (!tileMap.TryGetValue(tile, out var list) || list.Count == 0)
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

    public override (Texture2D, string) GetRandomDecoration(Random rng, int x, int y)
    {
        if (rng.NextDouble() < 0.3)
            return (GetSeeded(rocks, x, y), "rock");

        return (null, null);
    }

    public override float GetDecorationDensity() => 0.35f;

}