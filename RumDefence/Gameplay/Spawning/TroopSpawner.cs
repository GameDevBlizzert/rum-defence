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
    private Vector2 troopTargetDestination => grid.GridToWorld(level.RumTile);

    public void Update(GameTime gameTime)
    {
        Troop troop;
        if (!IsSpawning) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        spawnTimer += dt;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            troopsSpawned++;
            if (troopsSpawned < troopsToSpawn)
            {
                troop = new Troop("Art/Pirates/pirates-green-sprite-sheet", spawnPosition + GetSpawnOffset(), troopTargetDestination);
            }
            else
            {
                troop = new BossTroop("Art/Pirates/pirates-red-sprite-sheet", spawnPosition + GetSpawnOffset(), troopTargetDestination);
            }
            SpawnTroop(troop);
        }
    }

    private Vector2 GetSpawnOffset()
    {
        Vector2 offset = new Vector2(
            Random.Shared.Next(-10, 10),
            Random.Shared.Next(-10, 10)
        );
        return offset;
    }

    private void SpawnTroop(Troop troop)
    {
        SpawnedTroops.Add(troop);

        // Play random footstep sound when enemy spawns
        AudioManager.Instance.PlayRandomFootstep();
    }
}
