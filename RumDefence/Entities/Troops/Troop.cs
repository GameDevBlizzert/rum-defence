using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using RumDefence.Exceptions;

namespace RumDefence;

public class Troop : EntityWithHealth, ICollidable
{
    private Vector2 target;
    protected virtual Animation animation { get; set; }
    protected virtual Animation _swordAttackAnimation { get; set; }
    private Vector2 _lastDir = Vector2.UnitY;

    private float baseSpeed = 60f;
    public float SpeedMultiplier { get; set; } = 1f;
    public int CoinValue { get; set; } = 1;
    public bool HasDroppedReward { get; private set; }
    public bool IsFinished { get; private set; }

    private List<ITroopAbility> abilities = new();

    private static Texture2D pixel;

    private PathfindingSystem pathfinding;

    public Troop(string spritePath, Vector2 start, Vector2 targetPos) : base(16, 32)
    {
        Position = start;
        target = targetPos;
        animation = new TroopAnimation(
            16,
            16,
            0.2f,
            3,
            true
        );
        _swordAttackAnimation = new TroopSwordAttackAnimation();

        if (pixel == null)
        {
            pixel = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        // https://foozlecc.itch.io/scallywag-pirates
        Texture = RumGame.Instance.Content.Load<Texture2D>(spritePath);
        origin = Vector2.Zero;

        Size = SizeSystem.Square(10f);

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

    public override void Update(GameTime gameTime)
    {
        if (IsDead) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        SpeedMultiplier = 1f;

        foreach (var ability in abilities)
        {
            ability.Update(this, gameTime);
        }

        if (IsNearBarrel())
        {
            sourceRectangles = _swordAttackAnimation.GetCurrentLayerRectangles(gameTime, _lastDir);
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
        sourceRectangles = animation.GetCurrentLayerRectangles(gameTime, dir);

        Position += dir * speed * dt;
    }

    private bool IsNearBarrel()
    {
        var barrel = RumGame.Instance.CurrentLevel?.RumBarrel;
        if (barrel == null) return false;
        return Collider.CheckIntersection(barrel.Collider);
    }

    public void UpdatePathfinding(HashSet<Point> untraversableTiles)
    {
        pathfinding.UpdatePath(Position, RumGame.Instance.CurrentGrid, untraversableTiles);
    }

    public void MarkRewardGiven()
    {
        HasDroppedReward = true;
    }


    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

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
                    pixel,
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
                        pixel,
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
}