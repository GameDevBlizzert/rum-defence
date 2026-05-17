using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class NetEffect : Entity
{
    private float _lifeTime;
    private readonly float _maxLifeTime;

    public bool IsFinished => _lifeTime <= 0f;

    private static Texture2D _netTexture;

    public NetEffect(Vector2 position, float duration, float radius)
    {
        _lifeTime = duration;
        _maxLifeTime = duration;
        Position = position;

        if (_netTexture == null)
            _netTexture = RumGame.Instance.Content.Load<Texture2D>("Art/Towers/fishing-net");

        Texture = _netTexture;
        origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);

        // Texture is 256×128; scale to cover the AoE diameter while keeping 2:1 ratio
        float diameter = radius * 2f;
        Size = new Vector2(diameter, diameter / 2f);
        ApplySize();
    }

    public override void Update(GameTime gameTime)
    {
        _lifeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        color = Color.White * (_lifeTime / _maxLifeTime);
    }
}
