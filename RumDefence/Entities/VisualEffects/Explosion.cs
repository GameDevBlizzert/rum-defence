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
    private readonly Animation animation;
    public Explosion(Vector2 position, float radius)
    {
        _explosionIndex = new Random().Next(0, 3);
        // _explosionIndex = Math.Clamp(explosionIndex, 0, 2); // 0, 1, or 2
        _lifeTime = _maxLifeTime;
        Position = position;
        var SpriteFrameSize = 64;
        Texture = RumGame.Instance.Content.Load<Texture2D>("Art/Projectiles/explosion");
        var totalFrames = 36;
        animation = new Animation(SpriteFrameSize, SpriteFrameSize, _maxLifeTime / totalFrames);
        animation.AddLayerMatrix(
            [
                new(totalFrames - 1, SpriteAction.Static, SpriteDirection.None, isLoop: false)
            ]

        );
        animation.ActivateLayers([new(SpriteAction.Static, SpriteDirection.None)]);

        origin = new Vector2(SpriteFrameSize / 2f, SpriteFrameSize / 2f);

        float diameter = radius * 2f;
        Size = SizeSystem.Square(diameter);
        scale = Size.X / SpriteFrameSize;

        AudioManager.Instance.PlayRandomExplosion();
    }

    public override void Update(GameTime gameTime)
    {
        _lifeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Fade out alpha as the explosion expires
        float progress = 1 - (_lifeTime / _maxLifeTime);
        color = Color.White * (1 - progress);
        if (_lifeTime < 0) return;

        animation.Update(gameTime);

    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        DrawSpriteLayers(spriteBatch);
    }
    public void DrawSpriteLayers(SpriteBatch spriteBatch)
    {
        var items = animation.GetCurrentLayers();
        foreach (var item in items)
        {
            float itemRotation = item.Item1.Type == SpriteAction.Rotation ? rotation : 0f;
            spriteBatch.Draw(
                Texture,
                Position,
                item.Item2,
                color,
                itemRotation,
                origin,
                scale,
                item.Item1.Effect,
                item.Item1.Depth
            );
        }
    }
}
