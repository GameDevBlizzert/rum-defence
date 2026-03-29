using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RumDefence;

public abstract class BaseTheme : ITileTheme
{
    protected Random rng = Random.Shared; 

    protected Texture2D Load(string fullPath)
    {
        return RumGame.Instance.Content.Load<Texture2D>(fullPath);
    }

    protected List<Texture2D> LoadList(string prefix, params string[] names)
    {
        var list = new List<Texture2D>();

        foreach (var name in names)
            list.Add(Load(prefix + name));

        return list;
    }

    protected Texture2D GetRandom(List<Texture2D> list)
    {
        return list[rng.Next(list.Count)];
    }

    protected Texture2D GetSeeded(List<Texture2D> list, int x, int y)
    {
        if (list.Count == 1)
            return list[0];

        int seed = x * 73856093 ^ y * 19349663;
        var localRng = new Random(seed);

        return list[localRng.Next(list.Count)];
    }

    public abstract Texture2D GetTexture(int tile, int x, int y);
    public abstract Texture2D GetShip(string name);
    public abstract Texture2D GetRandomEnemy();
}