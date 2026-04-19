using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RumDefence;

public class ShipSpawner
{
    private readonly Level level;
    private readonly Grid grid;
    private readonly List<CoastTile> coastTiles;
    private readonly Random rng = new();

    // =====================
    // CURRENT WAVE (hold timer → attack coast)
    // =====================

    private int currentWaveIndex;
    private WavePhase phase;
    private List<ShipGroup> activeGroups;
    private List<Ship> currentWaveShips = new();
    private float spawnTimer;
    private float nextSpawnTime;

    // =====================
    // PRELOAD (next wave pre-spawns to holding while current attacks)
    // =====================

    private int preloadWaveIndex;
    private bool isPreloading;
    private List<ShipGroup> preloadGroups;
    private List<Ship> preloadShips = new();
    private float preloadSpawnTimer;
    private float preloadNextSpawnTime;

    // =====================
    // PUBLIC
    // =====================

    /// <summary>Ships spawned this frame — drain in GameScreen every update.</summary>
    public List<Ship> NewShips { get; } = new();

    /// <summary>True once the last wave has been set in motion and no preload remains.</summary>
    public bool IsAllWavesComplete { get; private set; }

    /// <summary>
    /// True while a preloaded wave exists (spawning or fully waiting at sea).
    /// GameScreen uses this to know when to call AdvancePreloadToAttack().
    /// </summary>
    public bool HasPreloadedWave => isPreloading || preloadShips.Count > 0;

    private enum WavePhase { Spawning, Holding, Attacking }

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
        currentWaveIndex = index;
        var wave = level.Waves[index];

        activeGroups = CopyGroups(wave.ShipGroups);
        currentWaveShips = new List<Ship>();
        phase = WavePhase.Spawning;
        spawnTimer = 0f;
        SetSpawnTime(wave, ref nextSpawnTime);
    }

    private void StartPreload(int index)
    {
        preloadWaveIndex = index;
        var wave = level.Waves[index];

        preloadGroups = CopyGroups(wave.ShipGroups);
        preloadShips = new List<Ship>();
        isPreloading = true;
        preloadSpawnTimer = 0f;
        SetSpawnTime(wave, ref preloadNextSpawnTime);
    }

    /// <summary>
    /// Called by GameScreen when the current attacking wave is cleared
    /// (no attacking ships + no troops alive).
    /// Advances preloaded ships to the coast and begins loading the wave after.
    /// </summary>
    private const float MinStaggerInterval = 2f;
    private const float MaxStaggerInterval = 6f;

    public void AdvancePreloadToAttack()
    {
        float cumulativeDelay = 0f;
        foreach (var s in preloadShips)
        {
            cumulativeDelay += MinStaggerInterval + (float)rng.NextDouble() * (MaxStaggerInterval - MinStaggerInterval);
            s.AdvanceToDock(cumulativeDelay);
        }

        preloadShips.Clear();
        isPreloading = false;

        int nextPreload = preloadWaveIndex + 1;
        if (nextPreload < level.Waves.Count)
            StartPreload(nextPreload);
        else
            IsAllWavesComplete = true;
    }

    // =====================
    // UPDATE
    // =====================

    public void Update(GameTime gameTime)
    {
        NewShips.Clear();

        if (IsAllWavesComplete) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var wave = level.Waves[currentWaveIndex];

        switch (phase)
        {
            case WavePhase.Spawning:
                UpdateCurrentWaveSpawning(dt, wave);
                break;

            case WavePhase.Holding:
                    ActivateCurrentWave();
                break;

            case WavePhase.Attacking:
                UpdatePreloadSpawning(dt);
                break;
        }
    }

    private void UpdateCurrentWaveSpawning(float dt, Wave wave)
    {
        spawnTimer += dt;
        if (spawnTimer < nextSpawnTime) return;
        spawnTimer = 0f;

        var ship = SpawnShip(activeGroups);
        if (ship == null) return;

        currentWaveShips.Add(ship);
        NewShips.Add(ship);

        if (activeGroups.All(g => g.Count <= 0))
        {
            phase = WavePhase.Holding;
        }
        else
        {
            SetSpawnTime(wave, ref nextSpawnTime);
        }
    }

    private void ActivateCurrentWave()
    {
        foreach (var s in currentWaveShips)
            s.AdvanceToDock();

        phase = WavePhase.Attacking;

        int nextPreload = currentWaveIndex + 1;
        if (nextPreload < level.Waves.Count)
            StartPreload(nextPreload);
        else
            IsAllWavesComplete = true; // single/last wave — done once ships clear
    }

    private void UpdatePreloadSpawning(float dt)
    {
        if (!isPreloading) return;

        preloadSpawnTimer += dt;
        var wave = level.Waves[preloadWaveIndex];
        if (preloadSpawnTimer < preloadNextSpawnTime) return;
        preloadSpawnTimer = 0f;

        var ship = SpawnShip(preloadGroups);
        if (ship == null) return;

        preloadShips.Add(ship);
        NewShips.Add(ship);

        if (preloadGroups.All(g => g.Count <= 0))
            isPreloading = false; // all preload ships spawned, now waiting at sea
        else
            SetSpawnTime(wave, ref preloadNextSpawnTime);
    }

    // =====================
    // HELPERS
    // =====================

    private const float MaxLateralOffset = 140f;

    private Ship SpawnShip(List<ShipGroup> groups)
    {
        var available = groups.FindAll(g => g.Count > 0);
        if (available.Count == 0) return null;

        var group = available[rng.Next(available.Count)];
        group.Count--;

        float lateral = ((float)rng.NextDouble() * 2f - 1f) * MaxLateralOffset;
        return (Ship)SpawnSystem.CreateShip(level, grid, group.Data, GetRandomCoast(), lateral);
    }

    private void SetSpawnTime(Wave wave, ref float target)
    {
        target = (float)(
            rng.NextDouble() * (wave.MaxSpawnTime - wave.MinSpawnTime)
            + wave.MinSpawnTime
        );
    }

    private List<ShipGroup> CopyGroups(List<ShipGroup> source)
    {
        var copy = new List<ShipGroup>();
        foreach (var g in source)
            copy.Add(new ShipGroup(g.Data, g.Count));
        return copy;
    }

    private CoastTile GetRandomCoast()
    {
        var occupied = new HashSet<Point>();
        foreach (var s in currentWaveShips)
            if (!s.IsFinished) occupied.Add(s.AssignedCoast.GridPos);
        foreach (var s in preloadShips)
            if (!s.IsFinished) occupied.Add(s.AssignedCoast.GridPos);

        var free = coastTiles.FindAll(c => !occupied.Contains(c.GridPos));
        var pool = free.Count > 0 ? free : coastTiles;
        return pool[rng.Next(pool.Count)];
    }
}
