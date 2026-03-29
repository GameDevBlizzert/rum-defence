using Microsoft.Xna.Framework;

namespace RumDefence;

public abstract class MovementComponent
{
    protected float speed;

    public MovementComponent(float speed)
    {
        this.speed = speed;
    }

    public abstract void Update(Entity entity, GameTime gameTime);

    public virtual bool HasArrived(Vector2 position) => false;
}