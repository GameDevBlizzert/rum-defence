using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class Ship : Entity
{
    public class Data
    {
        public string Texture;
        public float Speed;
        public int EnemyCount;
        public bool IsBoss;

        public Data(string texture, float speed, int enemyCount, bool isBoss = false)
        {
            Texture = texture;
            Speed = speed;
            EnemyCount = enemyCount;
            IsBoss = isBoss;
        }
    }

    private Vector2 target;
    private float speed;

    public int EnemyCount { get; private set; }
    public bool IsBoss { get; private set; }

    public bool HasArrived => Vector2.Distance(Position, target) < 5f;

    public Ship(Vector2 start, Vector2 target, Data data, Texture2D texture)
    {
        Position = start;
        this.target = target;

        speed = data.Speed;
        EnemyCount = data.EnemyCount;
        IsBoss = data.IsBoss;

        Texture = texture;
    }

    public override void Update(GameTime gameTime)
    {
        var dir = target - Position;

        if (dir != Vector2.Zero)
        {
            dir.Normalize();
            Position += dir * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }

    public Vector2 GetSpawnPosition()
    {
        return target;
    }
}