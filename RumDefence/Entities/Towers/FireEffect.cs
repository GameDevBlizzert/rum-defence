using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence;

public class FireEffect : Entity
{
    private float _lifeTime;
    private float _maxLifeTime = 0.6f;

    public bool IsFinished => _lifeTime <= 0;

    private static Texture2D[] _fireTextures;

    public FireEffect(Vector2 position, float radius)
    {
        _lifeTime = _maxLifeTime;
        Position = position;

        if (_fireTextures == null)
        {
            _fireTextures = new Texture2D[2];
            _fireTextures[0] = RumGame.Instance.Content.Load<Texture2D>("KenneyPiratePack/PNG/Retina/Effects/fire1");
            _fireTextures[1] = RumGame.Instance.Content.Load<Texture2D>("KenneyPiratePack/PNG/Retina/Effects/fire2");
        }

        Texture = _fireTextures[new Random().Next(0, 2)];
        origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
        float diameter = radius * 2f;
        Size = new Vector2(diameter, diameter);
        ApplySize();
    }

    public override void Update(GameTime gameTime)
    {
        _lifeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        float progress = 1 - (_lifeTime / _maxLifeTime);
        color = Color.White * (1 - progress);
    }
}
