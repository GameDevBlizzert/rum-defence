using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using RumDefence.Exceptions;

namespace RumDefence;

public class Troop : EntityWithHealth, ICollidable
{
    private Vector2 target;
    protected readonly Animation animation;
    private Vector2 _lastDir = Vector2.UnitY;

    private float baseSpeed;
    public float SpeedMultiplier { get; set; } = 1f;
    public float AttackSpeedMultiplier { get; set; } = 1f;
    public int CoinValue { get; set; } = 1;
    public bool HasDroppedReward { get; private set; }
    public bool IsFinished { get; private set; }

    public int Damage { get; set; } = 1;
    private float _attackTimer = 0f;

    private List<ITroopAbility> abilities = new();
    private readonly List<IModifier> _modifiers = new();


    public bool CanBeRemoved { get; private set; }
    public bool NeedsPathInit { get; protected set; } = true;
    protected virtual bool CanAttackWalls => true;
    protected PathfindingSystem pathfinding;
    public Queue<Vector2> Path => pathfinding?.Path;

    public Func<Point, Wall> GetWallAt { get; set; }

    public Troop(TroopData data, Vector2 start, Vector2 targetPos) : base(16, 32, data.Health)
    {
        Position = start;
        target = targetPos;

        baseSpeed = data.BaseSpeed;
        Damage = data.Damage;
        CoinValue = data.CoinValue;
        SpeedMultiplier = data.InitialSpeedMultiplier;

        // https://foozlecc.itch.io/scallywag-pirates
        animation = new Animation(16, 16, 0.2f);
        animation.AddLayerMatrix(
        [
            new(3, SpriteAction.Idle, SpriteDirection.Down),
            new(3, SpriteAction.Idle, SpriteDirection.Up),
            new(3, SpriteAction.Idle, SpriteDirection.Right),
            new(3, SpriteAction.Idle, SpriteDirection.Left),
            new(3, SpriteAction.Walking, SpriteDirection.Down),
            new(3, SpriteAction.Walking, SpriteDirection.Up),
            new(3, SpriteAction.Walking, SpriteDirection.Right),
            new(3, SpriteAction.Walking, SpriteDirection.Left),
            new(4, SpriteAction.Dying, SpriteDirection.Right, isLoop: false),
            new(3, SpriteAction.Attack, SpriteDirection.Down),
            new(3, SpriteAction.Attack, SpriteDirection.Up),
            new(3, SpriteAction.Attack, SpriteDirection.Right),
            new(3, SpriteAction.Attack, SpriteDirection.Left),
        ], 4);
        animation.ActivateLayers([new(SpriteAction.Idle, SpriteDirection.Down)]);

        Texture = RumGame.Instance.Content.Load<Texture2D>(data.SpritePath);
        origin = new Vector2(animation.FrameHeight / 2, animation.FrameWidth / 2);

        Size = SizeSystem.Square(data.Size);

        pathfinding = new(target);

        ApplySize();
    }

    public Collider Collider
    {
        get
        {
            var tileSize = RumGame.Instance.CurrentGrid?.TileSize ?? 32;
            return new CircleCollider(Position, tileSize * 0.75f);
        }
    }

    public void AddAbility(ITroopAbility ability)
    {
        abilities.Add(ability);
    }

    public void ApplyModifier(IModifier buff)
    {
        for (int i = 0; i < _modifiers.Count; i++)
        {
            if (_modifiers[i].GetType() == buff.GetType())
            {
                _modifiers[i].Refresh(buff);
                return;
            }
        }
        _modifiers.Add(buff);
    }

    public override void Update(GameTime gameTime)
    {
        animation.Update(gameTime);

        if (IsDead)
        {
            animation.ActivateLayers([new(SpriteAction.Dying, SpriteDirection.Right)]);
            if (animation.IsFinished)
                CanBeRemoved = true;
            return;
        }

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        SpeedMultiplier = 1f;
        AttackSpeedMultiplier = 1f;

        foreach (var ability in abilities)
        {
            ability.Update(this, gameTime);
        }

        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            _modifiers[i].Update(this, gameTime);
            if (_modifiers[i].IsExpired)
                _modifiers.RemoveAt(i);
        }

        if (IsNearBarrel())
        {
            animation.ActivateLayers([new(SpriteAction.Attack, VectorToSpriteDirection(_lastDir))]);
            _attackTimer += dt * AttackSpeedMultiplier;
            if (_attackTimer >= 1f)
            {
                RumGame.Instance.CurrentLevel?.RumBarrel?.TakeDamage(Damage);
                _attackTimer -= 1f;
            }
            return;
        }
        if (IsFinished) return;

        float speed = baseSpeed * SpeedMultiplier;

        var troopGridPos = RumGame.Instance.CurrentGrid.WorldToGrid(Position);
        var targetGridPos = RumGame.Instance.CurrentGrid.WorldToGrid(target);

        if (troopGridPos == null || targetGridPos == null || troopGridPos.Value == targetGridPos.Value)
        {
            IsFinished = true;
            return;
        }

        // If the next waypoint in the path is a wall, stop and attack it
        if (CanAttackWalls && GetWallAt != null && pathfinding.Path.Count > 0)
        {
            var grid = RumGame.Instance.CurrentGrid;
            var nextGrid = grid.WorldToGrid(pathfinding.Path.Peek());
            if (nextGrid.HasValue)
            {
                var wall = GetWallAt(nextGrid.Value);
                if (wall != null && !wall.IsDestroyed)
                {
                    var wallDir = pathfinding.Path.Peek() - Position;
                    if (wallDir != Vector2.Zero) wallDir.Normalize();
                    _lastDir = wallDir;
                    animation.ActivateLayers([new(SpriteAction.Attack, VectorToSpriteDirection(wallDir))]);
                    _attackTimer += dt * AttackSpeedMultiplier;
                    if (_attackTimer >= 1f)
                    {
                        wall.TakeDamage(Damage);
                        _attackTimer -= 1f;
                    }
                    return;
                }
            }
        }

        _attackTimer = 0f;

        Vector2 dir;

        try
        {
            dir = pathfinding.GetNextDirection(Position);
        }
        catch (NoPathPossibleException)
        {
            return;
        }

        dir.Normalize();
        _lastDir = dir;
        animation.ActivateLayers([new(SpriteAction.Walking, VectorToSpriteDirection(dir))]);

        Position += dir * speed * dt;
    }

    private static SpriteDirection VectorToSpriteDirection(Vector2 dir)
    {
        if (Math.Abs(dir.X) > Math.Abs(dir.Y))
            return dir.X > 0 ? SpriteDirection.Right : SpriteDirection.Left;
        return dir.Y > 0 ? SpriteDirection.Down : SpriteDirection.Up;
    }

    private bool IsNearBarrel()
    {
        var barrel = RumGame.Instance.CurrentLevel?.RumBarrel;
        if (barrel == null) return false;
        return Collider.CheckIntersection(barrel.Collider);
    }

    public virtual void UpdatePathfinding()
    {
        NeedsPathInit = false;
        var grid = RumGame.Instance.CurrentGrid;
        pathfinding.UpdatePath(Position, grid, grid.UntraversableTiles);
    }

    public void MarkRewardGiven()
    {
        HasDroppedReward = true;
    }


    public override void Draw(SpriteBatch spriteBatch)
    {
        DrawSpriteLayers(spriteBatch);
        DrawHealth(spriteBatch);

        bool showPathfindingDebug = bool.Parse(
            Environment.GetEnvironmentVariable("SHOW_PATHFINDING") ?? "false"
        );

        if (showPathfindingDebug)
        {
            Vector2 currentPos = Position;
            const int dotSize = 8;

            foreach (var point in pathfinding.Path)
            {
                spriteBatch.Draw(
                    Primitives.Pixel,
                    new Rectangle(
                        (int)(point.X - dotSize / 2f),
                        (int)(point.Y - dotSize / 2f),
                        dotSize,
                        dotSize
                    ),
                    Color.HotPink
                );

                Vector2 edge = point - currentPos;
                float length = edge.Length();

                if (length > 0.001f)
                {
                    float rotation = (float)Math.Atan2(edge.Y, edge.X);
                    spriteBatch.Draw(
                        Primitives.Pixel,
                        currentPos,
                        null,
                        Color.HotPink,
                        rotation,
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
    public void DrawSpriteLayers(SpriteBatch spriteBatch)
    {
        // if (IsFinished) return;
        var items = animation.GetCurrentLayers();
        foreach (var item in items)
        {
            float itemRotation = item.Item1.Type == SpriteAction.Rotation ? rotation : 0f;
            spriteBatch.Draw(
                Texture,
                Position,
                item.Item2,
                color,
                itemRotation,
                origin,
                scale,
                item.Item1.Effect,
                item.Item1.Depth
            );
        }
    }
}