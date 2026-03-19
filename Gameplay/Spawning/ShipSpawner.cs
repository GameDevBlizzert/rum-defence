using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class ShipSpawner
{
    private Level level;
    private List<Point> coastTiles;
    private Random rng = new Random();

    private int currentWaveIndex = 0;
    private float timer;
    private float nextSpawnTime;

    private List<ShipGroup> activeGroups;

    public bool IsFinished { get; private set; }

    public ShipSpawner(Level level)
    {
        this.level = level;
        coastTiles = CoastFinder.GetCoastTiles(level.Map);

        StartWave(0);
    }

    private void StartWave(int index)
    {
        var wave = level.Waves[index];

        activeGroups = new List<ShipGroup>();

        foreach (var g in wave.ShipGroups)
        {
            activeGroups.Add(new ShipGroup(g.Data, g.Count));
        }

        SetNextSpawnTime(wave);
    }

    private void SetNextSpawnTime(Wave wave)
    {
        nextSpawnTime = (float)(
            rng.NextDouble() * (wave.MaxSpawnTime - wave.MinSpawnTime)
            + wave.MinSpawnTime
        );
    }

    public Ship Update(GameTime gameTime)
    {
        if (currentWaveIndex >= level.Waves.Count)
        {
            IsFinished = true;
            return null;
        }

        timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        var wave = level.Waves[currentWaveIndex];

        if (timer >= nextSpawnTime)
        {
            timer = 0;

            var ship = SpawnFromWave(wave);

            SetNextSpawnTime(wave);

            return ship;
        }

        return null;
    }

    private Ship SpawnFromWave(Wave wave)
    {
        var available = activeGroups.FindAll(g => g.Count > 0);

        if (available.Count == 0)
        {
            currentWaveIndex++;

            if (currentWaveIndex < level.Waves.Count)
                StartWave(currentWaveIndex);
            else
                IsFinished = true;

            return null;
        }

        var group = available[rng.Next(available.Count)];
        group.Count--;

        var coast = coastTiles[rng.Next(coastTiles.Count)];

        Vector2 target = new Vector2(
            coast.X * 64 + 32,
            coast.Y * 64 + 32
        );

        Vector2 start = new Vector2(-200, target.Y);

        var texture = level.Theme.GetShip(group.Data.Texture);

        return new Ship(start, target, group.Data, texture);
    }
}