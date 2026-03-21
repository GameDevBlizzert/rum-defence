using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class Ship : Entity
{
    // =====================
    // DATA
    // =====================

    public class Data
    {
        public string Texture;
        public float Speed;
        public int EnemyCount;
        public bool IsBoss;
        public float SizeMultiplier;
        public float RotationOffset;
        public float WidthInTiles;

        public Data(
            string texture,
            float speed,
            int enemyCount,
            bool isBoss = false,
            float sizeMultiplier = 1f,
            float rotationOffsetDegrees = 0f,
            float widthInTiles = 1f
        )
        {
            Texture = texture;
            Speed = speed;
            EnemyCount = enemyCount;
            IsBoss = isBoss;
            SizeMultiplier = sizeMultiplier;
            RotationOffset = MathHelper.ToRadians(rotationOffsetDegrees);
            WidthInTiles = widthInTiles;
        }
    }

    private const float DockSlowdownDistance = 150f;
    private const float MinSpeedFactor = 0.2f;
    private const float RotationSpeed = 5f;
    private const float LeaveSpeed = 80f;

    private const float BackOffDistance = 100f;
    private const float ExitDistance = 400f;

    private enum ShipState
    {
        SailingToDock,
        Docked,
        Unloading,
        Leaving_BackOff,
        Leaving_ToSea
    }

    private ShipState state = ShipState.SailingToDock;

    private Vector2 dockTarget;
    private Vector2 leaveTarget;
    private Vector2 spawnPosition;

    private float baseSpeed;

    public int EnemyCount { get; private set; }

    public bool IsFinished => state == ShipState.Leaving_ToSea &&
                              Vector2.Distance(Position, leaveTarget) < 10f;

    // 🔥 TroopSpawner
    private TroopSpawner troopSpawner;

    public List<Troop> SpawnedTroops { get; } = new();

    // =====================
    // CONSTRUCTOR
    // =====================

    public Ship(Vector2 start, Vector2 target, Data data, Texture2D texture)
    {
        Position = start;
        spawnPosition = start;

        Texture = texture;
        origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);

        rotationOffset = data.RotationOffset;

        dockTarget = target;
        baseSpeed = data.Speed;

        EnemyCount = data.EnemyCount;

        Size = SizeSystem.FromTiles(data.WidthInTiles, data.WidthInTiles);
        ApplySize();
        scale *= data.SizeMultiplier;

        troopSpawner = new TroopSpawner(
            RumGame.Instance.CurrentLevel,
            RumGame.Instance.CurrentGrid
        );
    }

    // =====================
    // UPDATE
    // =====================

    public override void Update(GameTime gameTime)
    {
        switch (state)
        {
            case ShipState.SailingToDock:
                UpdateSailing(gameTime);
                break;

            case ShipState.Docked:
                StartUnloading();
                break;

            case ShipState.Unloading:
                UpdateUnloading(gameTime);
                break;

            case ShipState.Leaving_BackOff:
                UpdateLeavingBackOff(gameTime);
                break;

            case ShipState.Leaving_ToSea:
                UpdateLeavingToSea(gameTime);
                break;
        }
    }

    // =====================
    // SAILING
    // =====================

    private void UpdateSailing(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 dir = dockTarget - Position;
        float distance = dir.Length();

        if (dir != Vector2.Zero)
            dir.Normalize();

        float targetRotation = (float)Math.Atan2(dir.Y, dir.X);
        rotation = MathHelper.Lerp(rotation, targetRotation, RotationSpeed * dt);

        float speedFactor = MathHelper.Clamp(distance / DockSlowdownDistance, MinSpeedFactor, 1f);
        float currentSpeed = baseSpeed * speedFactor;

        Position += dir * currentSpeed * dt;

        if (distance < 5f)
            state = ShipState.Docked;
    }

    // =====================
    // UNLOADING
    // =====================

    private void StartUnloading()
    {
        troopSpawner.StartSpawning(Position, EnemyCount);
        state = ShipState.Unloading;
    }

    private void UpdateUnloading(GameTime gameTime)
    {
        troopSpawner.Update(gameTime);

        if (troopSpawner.SpawnedTroops.Count > 0)
        {
            SpawnedTroops.AddRange(troopSpawner.SpawnedTroops);
            troopSpawner.SpawnedTroops.Clear();
        }

        if (!troopSpawner.IsSpawning)
        {
            StartLeaving();
        }
    }

    // =====================
    // LEAVING
    // =====================

    private void StartLeaving()
    {
        Vector2 backward = GetForwardVector() * -1f;
        leaveTarget = Position + backward * BackOffDistance;

        state = ShipState.Leaving_BackOff;
    }

    private void UpdateLeavingBackOff(GameTime gameTime)
    {
        MoveTowards(leaveTarget, LeaveSpeed, gameTime);

        if (Vector2.Distance(Position, leaveTarget) < 10f)
        {
            Vector2 dirToSpawn = spawnPosition - dockTarget;
            dirToSpawn.Normalize();

            leaveTarget = spawnPosition + dirToSpawn * ExitDistance;

            state = ShipState.Leaving_ToSea;
        }
    }

    private void UpdateLeavingToSea(GameTime gameTime)
    {
        MoveTowards(leaveTarget, LeaveSpeed, gameTime);
    }

    // =====================
    // HELPERS
    // =====================

    private void MoveTowards(Vector2 target, float speed, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 dir = target - Position;

        if (dir != Vector2.Zero)
            dir.Normalize();

        float targetRotation = (float)Math.Atan2(dir.Y, dir.X);
        rotation = MathHelper.Lerp(rotation, targetRotation, RotationSpeed * dt);

        Position += dir * speed * dt;
    }

    private Vector2 GetForwardVector()
    {
        return new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
    }
}