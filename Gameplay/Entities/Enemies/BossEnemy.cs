using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class BossEnemy : Enemy
{
    public BossEnemy(Vector2 start, Texture2D texture)
        : base(start, texture)
    {
        Health = 300;
        Speed = 30f;
    }
}