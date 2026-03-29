using Microsoft.Xna.Framework;

namespace RumDefence;

public class GroundMovement : MovementComponent
{
    private Vector2 target;

    public GroundMovement(Vector2 target, float speed) : base(speed)
    {
        this.target = target;
    }

    public override void Update(Entity entity, GameTime gameTime)
    {
        var dir = target - entity.Position;

        if (dir != Vector2.Zero)
        {
            dir.Normalize();
            entity.Position += dir * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }

    public override bool HasArrived(Vector2 position)
    {
        return Vector2.Distance(position, target) < 5f;
    }
}