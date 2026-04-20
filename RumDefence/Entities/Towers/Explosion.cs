using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence;

public class Explosion : Entity
{
    private float _lifeTime;
    private float _maxLifeTime = 0.45f;
    private int _explosionIndex;

    public bool IsFinished => _lifeTime <= 0;

    private static Texture2D[] _explosionTextures;

    public Explosion(Vector2 position, int explosionIndex)
    {
        _explosionIndex = Math.Clamp(explosionIndex, 0, 2); // 0, 1, or 2
        _lifeTime = _maxLifeTime;
        Position = position;

        if (_explosionTextures == null)
        {
            _explosionTextures = new Texture2D[3];
            _explosionTextures[0] = RumGame.Instance.Content.Load<Texture2D>("KenneyPiratePack/PNG/Retina/Effects/explosion1");
            _explosionTextures[1] = RumGame.Instance.Content.Load<Texture2D>("KenneyPiratePack/PNG/Retina/Effects/explosion2");
            _explosionTextures[2] = RumGame.Instance.Content.Load<Texture2D>("KenneyPiratePack/PNG/Retina/Effects/explosion3");
        }

        Texture = _explosionTextures[_explosionIndex];
        origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
        Size = SizeSystem.Square(0.6f);
        ApplySize();
    }

    public override void Update(GameTime gameTime)
    {
        _lifeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Fade out alpha as the explosion expires
        float progress = 1 - (_lifeTime / _maxLifeTime);
        color = Color.White * (1 - progress);
    }
}
