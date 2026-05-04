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
    private readonly Random random = new Random();

    private int waveIndex = 0;
    private float spawnTimer = 0f;
    private float nextSpawnInterval = 0f;
    private int spawnCount = 0;

    private readonly Queue<Ship.Data> spawnQueue = new();

    public List<Ship> NewShips { get; } = new();

    public int CurrentWave => Math.Min(waveIndex + 1, TotalWaves);
    public int TotalWaves => waves.Count;
    public bool IsFinished => waveIndex >= waves.Count;
    public bool IsAllWavesComplete => IsFinished;

    public int TotalTroopsInWave { get; private set; }
    public int TroopsDefeatedInWave { get; private set; }
    public float WaveTroopProgress => TotalTroopsInWave > 0
        ? MathHelper.Clamp((float)TroopsDefeatedInWave / TotalTroopsInWave, 0f, 1f)
        : 0f;

    public ShipSpawner(Level level, Grid grid)
    {
        this.level = level;
        this.grid = grid;
        waves = level.Waves;
        coastTiles = CoastSystem.GetCoastTiles(level.Map);

        if (waves.Count > 0)
            StartSpawning();
    }

    public void NotifyTroopDefeated()
    {
        TroopsDefeatedInWave++;
    }

    public void Update(GameTime gameTime)
    {
        NewShips.Clear();
        if (IsFinished) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        spawnTimer += dt;
        if (spawnTimer >= nextSpawnInterval && spawnQueue.Count > 0)
        {
            SpawnNextShip();
            spawnTimer = 0f;
        }

        bool allShipsSpawned = spawnQueue.Count == 0;
        bool allTroopsDefeated = TotalTroopsInWave == 0 || TroopsDefeatedInWave >= TotalTroopsInWave;
        if (allShipsSpawned && allTroopsDefeated)
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
        TotalTroopsInWave = 0;
        TroopsDefeatedInWave = 0;

        var wave = waves[waveIndex];
        foreach (var group in wave.ShipGroups)
        {
            for (int i = 0; i < group.Count; i++)
                spawnQueue.Enqueue(group.Data);
            TotalTroopsInWave += group.Data.EnemyCount * group.Count;
        }
    }

    private void SpawnNextShip()
    {
        if (coastTiles.Count == 0) return;

        var data = spawnQueue.Dequeue();
        var coast = coastTiles[spawnCount % coastTiles.Count];
        float lateralOffset = (spawnCount / coastTiles.Count) * 30f;

        var ship = (Ship)SpawnSystem.CreateShip(level, grid, data, coast, lateralOffset);
        ship.AdvanceToDock(waves[waveIndex].HoldingTime);
        spawnCount++;

        var wave = waves[waveIndex];
        nextSpawnInterval = (float)(random.NextDouble() * (wave.MaxSpawnTime - wave.MinSpawnTime) + wave.MinSpawnTime);

        NewShips.Add(ship);
    }
}
