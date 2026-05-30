using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RumDefence;

public class BomberTroop : Troop
{
    private bool _hasExploded;
    private const float ExplosionRadius = 96f;
    private const float WallDamageModifier = 4f;
    public BomberTroop(TroopData data, Vector2 start, Vector2 target)
        : base(data, start, target)
    {
        animation.ResetLayerMatrix();
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
        ], 4);
        animation.ActivateLayers([new(SpriteAction.Idle, SpriteDirection.Down)]);
    }

    protected override bool CanAttackWalls => false;

    public override void UpdatePathfinding()
    {
        NeedsPathInit = false;
        var grid = RumGame.Instance.CurrentGrid;

        var waterOnly = new HashSet<Point>();
        for (int x = 0; x < grid.Width; x++)
            for (int y = 0; y < grid.Height; y++)
                if (TileRules.IsWater(grid.Tiles[y, x]))
                    waterOnly.Add(new Point(x, y));

        pathfinding.UpdatePath(Position, grid, waterOnly);
    }

    public override void Update(GameTime gameTime)
    {
        if (!IsDead && !_hasExploded)
        {
            var grid = RumGame.Instance.CurrentGrid;
            float triggerDist = grid?.TileSize ?? ExplosionRadius / 2 / 2;
            foreach (var wall in GameScreen.Instance.Walls)
            {
                if (Vector2.Distance(Position, grid.GridToWorld(wall.GridPos)) <= triggerDist)
                {
                    Explode();
                    TakeDamage(Health.Current + 1);
                    break;
                }
            }
            if (IsNearBarrel())
            {
                Health.TakeDamage(Health.Current);
                RumGame.Instance.CurrentLevel?.RumBarrel?.TakeDamage(Damage);
            }
        }

        if (IsDead && !_hasExploded)
            Explode();

        base.Update(gameTime);
    }

    private void Explode()
    {
        _hasExploded = true;
        GameScreen.Instance.Explosions.Add(new Explosion(Position, ExplosionRadius));

        var grid = RumGame.Instance.CurrentGrid;
        foreach (var wall in GameScreen.Instance.Walls)
        {
            var wallPos = grid.GridToWorld(wall.GridPos);
            float dist = Vector2.Distance(Position, wallPos);
            if (dist <= ExplosionRadius)
            {
                float falloff = 1f - (dist / ExplosionRadius);
                wall.TakeDamage((int)(Wall.MaxHealth * falloff * WallDamageModifier) + 1);
            }
        }
    }
}
