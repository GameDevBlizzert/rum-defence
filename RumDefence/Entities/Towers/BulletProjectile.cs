using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class BulletProjectile : BaseProjectile
{
    public BulletProjectile(Vector2 start, Troop target, float speed, int damage)
        : base(start, target, speed, damage)
    {
        Texture = RumGame.Instance.Content.Load<Texture2D>("KenneyPiratePack/PNG/Retina/Ship parts/cannonBall");
        origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
        // Size = SizeSystem.Square(0.25f);
        // ApplySize();
    }
}
