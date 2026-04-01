using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class TroopSpawner
{
    private Grid grid;
    private Level level;

    private float spawnTimer = 0f;
    private float spawnInterval = 1f;

    private int troopsToSpawn = 0;
    private int troopsSpawned = 0;

    private Vector2 spawnPosition;

    public List<Troop> SpawnedTroops { get; } = new();

    public bool IsSpawning => troopsSpawned < troopsToSpawn;

    public TroopSpawner(Level level, Grid grid)
    {
        this.level = level;
        this.grid = grid;
    }

    public void StartSpawning(Vector2 position, int count)
    {
        spawnPosition = position;
        troopsToSpawn = count;
        troopsSpawned = 0;
        spawnTimer = 0f;
    }

    public void Update(GameTime gameTime)
    {
        if (!IsSpawning) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        spawnTimer += dt;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            troopsSpawned++;

            SpawnTroop();
        }
    }

    private void SpawnTroop()
    {
        Vector2 target = grid.GridToWorld(level.RumTile);

        Vector2 offset = new Vector2(
            Random.Shared.Next(-10, 10),
            Random.Shared.Next(-10, 10)
        );

        var troop = new Troop(spawnPosition + offset, target);

        SpawnedTroops.Add(troop);
    }
}