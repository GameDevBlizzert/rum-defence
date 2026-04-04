using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class ShipSpawner
{
    private readonly Level level;
    private readonly Grid grid;
    private readonly List<CoastTile> coastTiles;
    private readonly Random rng = new();

    private int currentWaveIndex;
    private float timer;
    private float nextSpawnTime;

    private List<ShipGroup> activeGroups;

    public bool IsFinished { get; private set; }

    public ShipSpawner(Level level, Grid grid)
    {
        this.level = level;
        this.grid = grid;

        coastTiles = CoastSystem.GetCoastTiles(level.Map);

        StartWave(0);
    }

    // =====================
    // WAVE FLOW
    // =====================

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

    // =====================
    // UPDATE
    // =====================

    public Ship Update(GameTime gameTime)
    {
        if (IsFinished)
            return null;

        timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (timer < nextSpawnTime)
            return null;

        timer = 0f;

        var wave = level.Waves[currentWaveIndex];
        var ship = SpawnFromWave(wave);

        SetNextSpawnTime(wave);

        return ship;
    }

    // =====================
    // SPAWNING
    // =====================

    private Ship SpawnFromWave(Wave wave)
    {
        var group = GetNextGroup();

        if (group == null)
            return null;

        var coast = GetRandomCoast();

        return (Ship)SpawnSystem.CreateShip(level, grid, group.Data, coast
        );
    }

    // =====================
    // HELPERS
    // =====================

    private ShipGroup GetNextGroup()
    {
        var available = activeGroups.FindAll(g => g.Count > 0);

        if (available.Count > 0)
        {
            var group = available[rng.Next(available.Count)];
            group.Count--;
            return group;
        }

        currentWaveIndex++;

        if (currentWaveIndex < level.Waves.Count)
        {
            StartWave(currentWaveIndex);
            return GetNextGroup();
        }

        IsFinished = true;
        return null;
    }

    private CoastTile GetRandomCoast()
    {
        return coastTiles[rng.Next(coastTiles.Count)];
    }
}