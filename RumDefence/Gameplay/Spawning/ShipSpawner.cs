using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class ShipSpawner
{
    private readonly Level level;
    private readonly Grid grid;
    private readonly List<Wave> waves;
    private readonly List<CoastTile> coastTiles;

    private int waveIndex = 0;
    private float spawnTimer = 0f;
    private float nextSpawnInterval = 0f;
    private float spawnInterval = 0f;
    private int spawnCount = 0;
    private float waveElapsed = 0f;

    private readonly Queue<Ship.Data> spawnQueue = new();

    public List<Ship> NewShips { get; } = new();

    public int CurrentWave => Math.Min(waveIndex + 1, TotalWaves);
    public int TotalWaves => waves.Count;
    public float WaveDuration => IsFinished ? 0f : waves[waveIndex].WaveDuration;
    public float WaveTimeRemaining => IsFinished ? 0f : Math.Max(0f, WaveDuration - waveElapsed);
    public bool IsFinished => waveIndex >= waves.Count;
    public bool IsAllWavesComplete => IsFinished;

    public ShipSpawner(Level level, Grid grid)
    {
        this.level = level;
        this.grid = grid;
        waves = level.Waves;
        coastTiles = CoastSystem.GetCoastTiles(level.Map);

        if (waves.Count > 0)
            StartSpawning();
    }

    public void Update(GameTime gameTime)
    {
        NewShips.Clear();
        if (IsFinished) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        waveElapsed += dt;
        spawnTimer += dt;
        if (spawnTimer >= nextSpawnInterval && spawnQueue.Count > 0)
        {
            SpawnNextShip();
            spawnTimer = 0f;
            nextSpawnInterval = spawnInterval;
        }

        if (spawnQueue.Count == 0)
        {
            waveIndex++;
            if (waveIndex < waves.Count)
                StartSpawning();
        }
    }

    private void StartSpawning()
    {
        spawnQueue.Clear();
        spawnCount = 0;
        spawnTimer = 0f;
        nextSpawnInterval = 0f;
        waveElapsed = 0f;

        var wave = waves[waveIndex];
        foreach (var group in wave.ShipGroups)
            for (int i = 0; i < group.Count; i++)
                spawnQueue.Enqueue(group.Data);

        int totalShips = spawnQueue.Count;
        spawnInterval = totalShips > 0 ? wave.WaveDuration / totalShips : 0f;
    }

    private void SpawnNextShip()
    {
        if (coastTiles.Count == 0) return;

        var data = spawnQueue.Dequeue();
        var coast = coastTiles[spawnCount % coastTiles.Count];
        float lateralOffset = (spawnCount / coastTiles.Count) * 30f;

        var ship = (Ship)SpawnSystem.CreateShip(level, grid, data, coast, lateralOffset);
        ship.AdvanceToDock();
        spawnCount++;

        NewShips.Add(ship);
    }
}
