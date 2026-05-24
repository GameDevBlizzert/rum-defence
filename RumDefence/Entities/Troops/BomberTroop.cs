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
        // _walkanimation = new BomberTroopAnimation(16, 16, 0.2f, true);
        // _dyingAnimation = new BomberTroopDyingAnimation();
    }

    protected override bool CanAttackWalls => false;

    // public override void UpdatePathfinding()
    // {
    //     NeedsPathInit = false;
    //     var grid = RumGame.Instance.CurrentGrid;
    //     var walls = GameScreen.Instance.Walls;

    //     Wall nearest = null;
    //     float nearestDist = float.MaxValue;
    //     foreach (var wall in walls)
    //     {
    //         float dist = Vector2.Distance(Position, grid.GridToWorld(wall.GridPos));
    //         if (dist < nearestDist)
    //         {
    //             nearestDist = dist;
    //             nearest = wall;
    //         }
    //     }

    //     if (nearest != null)
    //     {
    //         pathfinding = new PathfindingSystem(grid.GridToWorld(nearest.GridPos));
    //         pathfinding.UpdatePath(Position, grid, null);
    //     }
    //     else
    //     {
    //         pathfinding.UpdatePath(Position, grid, grid.UntraversableTiles);
    //     }
    // }

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
                    TakeDamage(Health + 1);
                    break;
                }
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
