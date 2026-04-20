using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.Exceptions;
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

    private const float BackOffDistance = 1f;
    private const float ExitDistance = 400f;

    public enum ShipState
    {
        SailingToHoldingPosition,
        HoldingAtSea,
        SailingToDock,
        Docked,
        Unloading,
        Leaving_BackOff,
        Leaving_ToSea
    }

    public ShipState State { get; private set; } = ShipState.SailingToHoldingPosition;

    private Vector2 holdingPosition;
    private Vector2 dockTarget;
    private Vector2 leaveTarget;
    private Vector2 spawnPosition;

    private float baseSpeed;
    private float advanceDelay;

    private Grid grid;
    private PathfindingSystem pathfinding;

    public int EnemyCount { get; private set; }
    public CoastTile AssignedCoast { get; private set; }

    public bool IsFinished => State == ShipState.Leaving_ToSea &&
                              Vector2.Distance(Position, leaveTarget) < 10f;

    private TroopSpawner troopSpawner;

    public List<Troop> SpawnedTroops { get; } = new();

    // =====================
    // CONSTRUCTOR
    // =====================

    public Ship(Vector2 start, Vector2 holding, Vector2 target, CoastTile coast, Data data, Texture2D texture)
    {
        Position = start;
        spawnPosition = start;

        Texture = texture;
        origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);

        rotationOffset = data.RotationOffset;

        AssignedCoast = coast;
        holdingPosition = holding;
        dockTarget = target;
        baseSpeed = data.Speed;

        EnemyCount = data.EnemyCount;

        Size = SizeSystem.FromTiles(data.WidthInTiles, data.WidthInTiles);
        ApplySize();
        scale *= data.SizeMultiplier;

        grid = RumGame.Instance.CurrentGrid;

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
        switch (State)
        {
            case ShipState.SailingToHoldingPosition:
                UpdateSailingToHolding(gameTime);
                break;

            case ShipState.HoldingAtSea:
                UpdateHoldingAtSea(gameTime);
                break;

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
    // HOLDING
    // =====================

    private void UpdateSailingToHolding(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 dir = holdingPosition - Position;
        float distance = dir.Length();

        if (dir != Vector2.Zero)
            dir.Normalize();

        float targetRotation = (float)Math.Atan2(dir.Y, dir.X);
        rotation = MathHelper.Lerp(rotation, targetRotation, RotationSpeed * dt);

        Position += dir * baseSpeed * dt;

        if (distance < 5f)
            State = ShipState.HoldingAtSea;
    }

    public void AdvanceToDock(float delay = 0f)
    {
        if (State != ShipState.HoldingAtSea && State != ShipState.SailingToHoldingPosition)
            return;

        if (delay <= 0f)
        {
            State = ShipState.SailingToDock;
        }
        else
        {
            advanceDelay = delay;
            // ship stays in HoldingAtSea (or finishes sailing there) and ticks down
        }
    }

    private void UpdateHoldingAtSea(GameTime gameTime)
    {
        if (advanceDelay <= 0f) return;

        advanceDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (advanceDelay <= 0f)
            State = ShipState.SailingToDock;
    }

    // =====================
    // SAILING
    // =====================

    private void UpdateSailing(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Initialize pathfinding once the ship has entered the grid
        if (pathfinding == null && grid.WorldToGrid(Position) != null)
            InitShipPathfinding();

        Vector2 dir;

        if (pathfinding != null)
        {
            try
            {
                dir = pathfinding.GetNextDirection(Position);
            }
            catch (NoPathPossibleException)
            {
                State = ShipState.Docked;
                return;
            }

            if (dir == Vector2.Zero)
            {
                State = ShipState.Docked;
                return;
            }
        }
        else
        {
            // Still off-screen: move directly toward the dock
            dir = dockTarget - Position;
            if (dir.Length() < 5f)
            {
                State = ShipState.Docked;
                return;
            }
            dir.Normalize();
        }

        float targetRotation = (float)Math.Atan2(dir.Y, dir.X);
        rotation = MathHelper.Lerp(rotation, targetRotation, RotationSpeed * dt);

        float distance = Vector2.Distance(Position, dockTarget);
        float speedFactor = MathHelper.Clamp(distance / DockSlowdownDistance, MinSpeedFactor, 1f);
        float currentSpeed = baseSpeed * speedFactor;

        Position += dir * currentSpeed * dt;
    }

    private const float PathNoiseDensity = 0.20f;

    private void InitShipPathfinding()
    {
        var rng = new Random();

        // Untraversable for ships = all non-water tiles
        var untraversable = new HashSet<Point>();
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (!TileRules.IsWater(grid.Tiles[y, x]))
                {
                    untraversable.Add(new Point(x, y));
                }
                else if (rng.NextDouble() < PathNoiseDensity)
                {
                    // Randomly block some water tiles to break up straight paths
                    untraversable.Add(new Point(x, y));
                }
            }
        }

        // The dock tile and the ship's current entry tile must stay reachable
        var dockGridPos = grid.WorldToGrid(dockTarget);
        if (dockGridPos.HasValue)
            untraversable.Remove(dockGridPos.Value);

        var entryGridPos = grid.WorldToGrid(Position);
        if (entryGridPos.HasValue)
            untraversable.Remove(entryGridPos.Value);

        pathfinding = new PathfindingSystem(dockTarget);

        // Fall back to a straight path if noise accidentally blocked all routes
        if (!TryUpdatePath(Position, untraversable))
        {
            untraversable.RemoveWhere(p => TileRules.IsWater(grid.Tiles[p.Y, p.X]));
            grid.UntraversableTiles = untraversable;
            pathfinding.UpdatePath(Position, grid);
        }
    }

    private bool TryUpdatePath(Vector2 position, HashSet<Point> untraversable)
    {
        grid.UntraversableTiles = untraversable;
        pathfinding.UpdatePath(position, grid);
        return pathfinding.Path.Count > 0;
    }

    // =====================
    // UNLOADING
    // =====================

    private void StartUnloading()
    {
        troopSpawner.StartSpawning(Position, EnemyCount);
        State = ShipState.Unloading;
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

        State = ShipState.Leaving_BackOff;
    }

    private void UpdateLeavingBackOff(GameTime gameTime)
    {
        MoveTowards(leaveTarget, LeaveSpeed, gameTime);

        if (Vector2.Distance(Position, leaveTarget) < 10f)
        {
            Vector2 exitDir = Position - dockTarget;
            if (exitDir != Vector2.Zero) exitDir.Normalize();

            leaveTarget = GetScreenExitPoint(Position, exitDir);

            State = ShipState.Leaving_ToSea;
        }
    }

    private Vector2 GetScreenExitPoint(Vector2 from, Vector2 dir)
    {
        float tMin = float.MaxValue;

        if (dir.X != 0)
        {
            float t = dir.X < 0
                ? -from.X / dir.X
                : (RumGame.VirtualWidth - from.X) / dir.X;
            if (t > 0) tMin = Math.Min(tMin, t);
        }

        if (dir.Y != 0)
        {
            float t = dir.Y < 0
                ? -from.Y / dir.Y
                : (RumGame.VirtualHeight - from.Y) / dir.Y;
            if (t > 0) tMin = Math.Min(tMin, t);
        }

        return from + dir * tMin;
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
