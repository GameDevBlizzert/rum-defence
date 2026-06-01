using System;
using System.Collections.Generic;

namespace RumDefence;

public class InfiniteWaveConfig
{
    // ── Ship types used when generating waves ──────────────────────────────
    public required Ship.Data NormalShip { get; init; }
    public required Ship.Data BossShip   { get; init; }

    // ── Base values for the very first generated wave (generationIndex = 0) ─
    public int   BaseShipCount        { get; init; } = 3;
    public int   BaseTroopCountPerShip { get; init; } = 5;
    public float BaseTroopSpawnDelay   { get; init; } = 0.8f;
    public float BaseMinSpawnTime      { get; init; } = 8f;
    public float BaseMaxSpawnTime      { get; init; } = 14f;

    // ── Per-wave multiplicative scaling ────────────────────────────────────
    // All scaling is applied as:  baseValue * factor^generationIndex
    public float HealthScalePerWave    { get; init; } = 1.15f;  // troop HP per wave
    public float ShipCountScalePerWave { get; init; } = 1.10f;  // ships per wave
    public float TroopCountScalePerWave{ get; init; } = 1.10f;  // troops per ship per wave
    public float SpawnTimeScalePerWave { get; init; } = 0.93f;  // < 1 → shorter gaps → harder

    // ── Floors to keep timing sane at high wave counts ─────────────────────
    public float MinSpawnTimeFloor { get; init; } = 0.5f;
    public float MaxSpawnTimeFloor { get; init; } = 2.0f;

    // ── Hard caps so waves stay playable at extreme counts ─────────────────
    public int MaxShipCount         { get; init; } = 12;
    public int MaxTroopCountPerShip { get; init; } = 15;

    // ── Ghost troop settings ───────────────────────────────────────────────
    public int   GhostWaveInterval      { get; init; } = 4;    // every Nth generated wave
    public float GhostTroopCountRatio   { get; init; } = 0.6f; // ghost count = regular * ratio

    // ── Bomber troop settings ──────────────────────────────────────────────
    public int   BomberWaveInterval     { get; init; } = 3;
    public float BomberTroopCountRatio  { get; init; } = 0.5f;

    // ── Boss wave settings ─────────────────────────────────────────────────
    public int   BossWaveInterval           { get; init; } = 5;
    public int   BaseBossCount              { get; init; } = 1;
    public float BossCountScalePerBossWave  { get; init; } = 1.3f; // more bosses each boss wave

    // ──────────────────────────────────────────────────────────────────────
    // Generates a wave for the given generation index (0 = first infinite wave).
    // Seed waves from the level data are not counted; the spawner tracks its own counter.
    // ──────────────────────────────────────────────────────────────────────
    public Wave GenerateWave(int generationIndex)
    {
        float Scaled(float baseVal, float scalePerWave)
            => baseVal * (float)Math.Pow(scalePerWave, generationIndex);

        int shipCount  = Math.Min(MaxShipCount,         Math.Max(1, (int)Math.Round(Scaled(BaseShipCount,         ShipCountScalePerWave))));
        int troopCount = Math.Min(MaxTroopCountPerShip, Math.Max(1, (int)Math.Round(Scaled(BaseTroopCountPerShip, TroopCountScalePerWave))));
        int regularHp  = Math.Max(1, (int)Math.Round(TroopFactory.Regular.Health * Math.Pow(HealthScalePerWave, generationIndex)));

        float minSpawn = Math.Max(MinSpawnTimeFloor, Scaled(BaseMinSpawnTime, SpawnTimeScalePerWave));
        float maxSpawn = Math.Max(MaxSpawnTimeFloor, Scaled(BaseMaxSpawnTime, SpawnTimeScalePerWave));

        int waveNumber = generationIndex + 1; // 1-indexed for interval checks

        var groups = new List<ShipGroup>();

        // Regular troops — always present
        groups.Add(new ShipGroup(
            NormalShip,
            shipCount,
            new List<TroopGroup> { new(TroopFactory.Regular with { Health = regularHp }, troopCount) },
            BaseTroopSpawnDelay));

        // Ghost troops
        if (waveNumber % GhostWaveInterval == 0)
        {
            int ghostHp    = Math.Max(1, (int)Math.Round(TroopFactory.Ghost.Health * Math.Pow(HealthScalePerWave, generationIndex)));
            int ghostCount = Math.Max(1, (int)Math.Round(troopCount * GhostTroopCountRatio));
            groups.Add(new ShipGroup(
                NormalShip,
                Math.Max(1, shipCount / 2),
                new List<TroopGroup> { new(TroopFactory.Ghost with { Health = ghostHp }, ghostCount) },
                BaseTroopSpawnDelay * 0.5f));
        }

        // Bomber troops
        if (waveNumber % BomberWaveInterval == 0)
        {
            int bomberHp    = Math.Max(1, (int)Math.Round(TroopFactory.Bomber.Health * Math.Pow(HealthScalePerWave, generationIndex)));
            int bomberCount = Math.Max(1, (int)Math.Round(troopCount * BomberTroopCountRatio));
            groups.Add(new ShipGroup(
                NormalShip,
                Math.Max(1, shipCount / 2),
                new List<TroopGroup> { new(TroopFactory.Bomber with { Health = bomberHp }, bomberCount) },
                BaseTroopSpawnDelay * 0.7f));
        }

        // Boss
        if (waveNumber % BossWaveInterval == 0)
        {
            int bossWaveNumber = waveNumber / BossWaveInterval;
            int bossCount = Math.Max(1, (int)Math.Round(BaseBossCount * Math.Pow(BossCountScalePerBossWave, bossWaveNumber - 1)));
            int bossHp    = Math.Max(1, (int)Math.Round(TroopFactory.Boss.Health * Math.Pow(HealthScalePerWave, generationIndex)));
            groups.Add(new ShipGroup(
                BossShip,
                1,
                new List<TroopGroup> { new(TroopFactory.Boss with { Health = bossHp }, bossCount) },
                0.1f));
        }

        return new Wave(groups, minSpawn, maxSpawn);
    }
}
