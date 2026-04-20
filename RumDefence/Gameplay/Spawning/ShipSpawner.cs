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

    private const float WaveInterval = 30f;
    private float waveCountdown;
    private bool inCountdown;

    public int CurrentWave => currentWaveIndex + 1;
    public int TotalWaves { get; private set; }
    public float WaveCountdown => waveCountdown;
    public bool IsInCountdown => inCountdown;
    public bool IsFinished { get; private set; }

    public ShipSpawner(Level level, Grid grid)
    {
        this.level = level;
        this.grid = grid;

        coastTiles = CoastSystem.GetCoastTiles(level.Map);
        TotalWaves = level.Waves.Count;

        BeginCountdown();
    }

    // =====================
    // WAVE FLOW
    // =====================

    private void BeginCountdown()
    {
        inCountdown = true;
        waveCountdown = WaveInterval;
    }

    private void StartWave(int index)
    {
        inCountdown = false;
        timer = 0f;

        var wave = level.Waves[index];
        activeGroups = new List<ShipGroup>();

        foreach (var g in wave.ShipGroups)
            activeGroups.Add(new ShipGroup(g.Data, g.Count));

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

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (inCountdown)
        {
            waveCountdown -= dt;
            if (waveCountdown <= 0f)
                StartWave(currentWaveIndex);
            return null;
        }

        timer += dt;

        if (timer < nextSpawnTime)
            return null;

        timer = 0f;

        var wave = level.Waves[currentWaveIndex];
        var ship = SpawnFromWave(wave);

        if (!inCountdown && !IsFinished)
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

        return (Ship)SpawnSystem.CreateShip(level, grid, group.Data, coast);
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
            BeginCountdown();
            return null;
        }

        IsFinished = true;
        return null;
    }

    private CoastTile GetRandomCoast()
    {
        return coastTiles[rng.Next(coastTiles.Count)];
    }
}
