using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RumDefence;

public class Ship : Entity
{
    // =====================
    // DATA
    // =====================

    public record Data
    {
        public string Texture { get; init; } = "";
        public float Speed { get; init; }
        public bool IsBoss { get; init; } = false;
        public float SizeMultiplier { get; init; } = 1f;
        public float RotationOffsetDegrees { get; init; } = 0f;
        public float WidthInTiles { get; init; } = 1f;
    }

    private const float DockSlowdownDistance = 150f;
    private const float MinSpeedFactor = 0.2f;
    private const float RotationSpeed = 5f;
    private const float LeaveSpeed = 80f;

    private const float BackOffDistance = 1f;

    public enum ShipState
    {
        SailingToDock,
        Docked,
        Unloading,
        Leaving_BackOff,
        Leaving_ToSea
    }

    public ShipState State { get; private set; } = ShipState.SailingToDock;

    private Vector2 dockTarget;
    private Vector2 leaveTarget;
    private Vector2 spawnPosition;

    private float baseSpeed;
    private float troopSpawnDelay;

    private Grid grid;
    private PathfindingSystem pathfinding;
    private List<Vector2> inboundPath = new List<Vector2>();
    private Queue<Vector2> leavingPath = new Queue<Vector2>();

    public IReadOnlyList<TroopGroup> Troops { get; private set; }
    public CoastTile AssignedCoast { get; private set; }

    public bool IsFinished => State == ShipState.Leaving_ToSea &&
                              Vector2.Distance(Position, spawnPosition) < 10f;

    private TroopSpawner troopSpawner;

    public List<Troop> SpawnedTroops { get; } = new();

    // =====================
    // CONSTRUCTOR
    // =====================

    public Ship(Vector2 start, Vector2 target, CoastTile coast, Data data, Texture2D texture, IReadOnlyList<TroopGroup> troops, float troopSpawnDelay)
    {
        Position = start;
        spawnPosition = start;

        Texture = texture;
        origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);

        rotationOffset = MathHelper.ToRadians(data.RotationOffsetDegrees);

        AssignedCoast = coast;
        dockTarget = target;
        baseSpeed = data.Speed;

        Troops = troops;
        this.troopSpawnDelay = troopSpawnDelay;

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
        // Initialize pathfinding once the ship has entered the grid
        if (pathfinding == null && grid.WorldToGrid(Position) != null)
            InitShipPathfinding();

        Vector2 dir;

        if (pathfinding != null)
        {
            float distance = Vector2.Distance(Position, dockTarget);
            float speedFactor = MathHelper.Clamp(distance / DockSlowdownDistance, MinSpeedFactor, 1f);
            float currentSpeed = baseSpeed * speedFactor;

            if (MoveAlongPath(pathfinding.Path, currentSpeed, gameTime, out dir))
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
        if (pathfinding == null)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            rotation += MathHelper.WrapAngle(targetRotation - rotation) * RotationSpeed * dt;

            float distance = Vector2.Distance(Position, dockTarget);
            float speedFactor = MathHelper.Clamp(distance / DockSlowdownDistance, MinSpeedFactor, 1f);
            float currentSpeed = baseSpeed * speedFactor;

            Position += dir * currentSpeed * dt;
        }
    }

    private const float PathNoiseDensity = 0.20f;

    private void InitShipPathfinding()
    {
        var untraversable = BuildShipUntraversable();

        var dockGridPos = grid.WorldToGrid(dockTarget);
        if (dockGridPos.HasValue)
            untraversable.Remove(dockGridPos.Value);

        var entryGridPos = grid.WorldToGrid(Position);
        if (entryGridPos.HasValue)
            untraversable.Remove(entryGridPos.Value);

        pathfinding = new PathfindingSystem(dockTarget);
        pathfinding.UpdatePath(Position, grid, untraversable);
        inboundPath = pathfinding.Path.ToList();

        // Fall back without noise if noise accidentally blocked all routes
        if (pathfinding.Path.Count == 0)
        {
            untraversable.RemoveWhere(p => TileRules.IsWater(grid.Tiles[p.Y, p.X]));
            pathfinding.UpdatePath(Position, grid, untraversable);
            inboundPath = pathfinding.Path.ToList();
        }
    }

    private HashSet<Point> BuildShipUntraversable()
    {
        var untraversable = new HashSet<Point>();
        for (int x = 0; x < grid.Width; x++)
            for (int y = 0; y < grid.Height; y++)
            {
                if (!TileRules.IsWater(grid.Tiles[y, x]))
                    untraversable.Add(new Point(x, y));
                else if (Random.Shared.NextDouble() < PathNoiseDensity)
                    untraversable.Add(new Point(x, y));
            }
        return untraversable;
    }

    private bool MoveAlongPath(Queue<Vector2> path, float speed, GameTime gameTime, out Vector2 direction)
    {
        direction = Vector2.Zero;

        if (path == null || path.Count == 0)
            return true;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        while (path.Count > 0 && Vector2.Distance(Position, path.Peek()) < 5f)
            path.Dequeue();

        if (path.Count == 0)
            return true;

        direction = path.Peek() - Position;

        if (direction != Vector2.Zero)
            direction.Normalize();

        float targetRotation = (float)Math.Atan2(direction.Y, direction.X);
        rotation += MathHelper.WrapAngle(targetRotation - rotation) * RotationSpeed * dt;

        Position += direction * speed * dt;
        return false;
    }

    // =====================
    // UNLOADING
    // =====================

    private void StartUnloading()
    {
        troopSpawner.StartSpawning(Position, Troops, troopSpawnDelay);
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

        leavingPath = new Queue<Vector2>(inboundPath.AsEnumerable().Reverse());
        leavingPath.Enqueue(spawnPosition);

        State = ShipState.Leaving_BackOff;
    }

    private void UpdateLeavingBackOff(GameTime gameTime)
    {
        MoveTowards(leaveTarget, LeaveSpeed, gameTime);

        if (Vector2.Distance(Position, leaveTarget) < 10f)
        {
            State = ShipState.Leaving_ToSea;
        }
    }

    private void UpdateLeavingToSea(GameTime gameTime)
    {
        MoveAlongPath(leavingPath, LeaveSpeed, gameTime, out _);
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
        rotation += MathHelper.WrapAngle(targetRotation - rotation) * RotationSpeed * dt;

        Position += dir * speed * dt;
    }

    private Vector2 GetForwardVector()
    {
        return new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
    }

    // =====================
    // DRAW
    // =====================

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        bool showPathfindingDebug = bool.Parse(
            Environment.GetEnvironmentVariable("SHOW_PATHFINDING") ?? "false"
        );

        if (showPathfindingDebug)
        {
            DrawPathDebug(spriteBatch, pathfinding?.Path, Color.Cyan);
            DrawPathDebug(spriteBatch, leavingPath, Color.Yellow);
        }
    }

    private void DrawPathDebug(SpriteBatch spriteBatch, IEnumerable<Vector2> path, Color color)
    {
        if (path == null) return;

        Vector2 currentPos = Position;
        const int dotSize = 8;

        foreach (var point in path)
        {
            spriteBatch.Draw(
                Primitives.Pixel,
                new Rectangle(
                    (int)(point.X - dotSize / 2f),
                    (int)(point.Y - dotSize / 2f),
                    dotSize,
                    dotSize
                ),
                color
            );

            Vector2 edge = point - currentPos;
            float length = edge.Length();

            if (length > 0.001f)
            {
                float rot = (float)Math.Atan2(edge.Y, edge.X);
                spriteBatch.Draw(
                    Primitives.Pixel,
                    currentPos,
                    null,
                    color,
                    rot,
                    Vector2.Zero,
                    new Vector2(length, 2f),
                    SpriteEffects.None,
                    0f
                );
            }

            currentPos = point;
        }
    }
}
