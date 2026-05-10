using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class TroopSpawner
{
    private Grid grid;
    private Level level;

    private readonly Queue<TroopData> troopQueue = new();
    private float spawnTimer = 0f;
    private float spawnInterval = 1f;
    private Vector2 spawnPosition;

    public List<Troop> SpawnedTroops { get; } = new();

    public bool IsSpawning => troopQueue.Count > 0;

    public TroopSpawner(Level level, Grid grid)
    {
        this.level = level;
        this.grid = grid;
    }

    public void StartSpawning(Vector2 position, IReadOnlyList<TroopGroup> troops, float spawnDelay = 1f)
    {
        spawnPosition = position;
        troopQueue.Clear();
        spawnTimer = 0f;
        spawnInterval = spawnDelay;

        foreach (var group in troops)
            for (int i = 0; i < group.Count; i++)
                troopQueue.Enqueue(group.Data);
    }

    private Vector2 troopTargetDestination => grid.GridToWorld(level.RumTile);

    public void Update(GameTime gameTime)
    {
        if (!IsSpawning) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        spawnTimer += dt;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            var data = troopQueue.Dequeue();
            SpawnTroop(TroopFactory.Create(data, spawnPosition + GetSpawnOffset(), troopTargetDestination));
        }
    }

    private Vector2 GetSpawnOffset()
    {
        return new Vector2(
            Random.Shared.Next(-10, 10),
            Random.Shared.Next(-10, 10)
        );
    }

    private void SpawnTroop(Troop troop)
    {
        SpawnedTroops.Add(troop);
        AudioManager.Instance.PlayRandomFootstep();
    }
}
