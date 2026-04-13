using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence;

public class Coin
{
    private Vector2 position;
    private Vector2 startPosition;

    private Func<Vector2> getTarget;

    private Texture2D texture;

    private float progress = 0f;
    private float duration = 2f;

    private float scale = 0.01f;

    public bool IsFinished { get; private set; }

    public Coin(Vector2 start, Func<Vector2> getTarget, Texture2D texture)
    {
        this.position = start;
        this.startPosition = start;
        this.getTarget = getTarget;
        this.texture = texture;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        progress += dt / duration;

        if (progress >= 1f)
        {
            IsFinished = true;
            return;
        }

        Vector2 target = getTarget();

        float t = progress * progress * (3f - 2f * progress);

        position = Vector2.Lerp(startPosition, target, t);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            texture,
            position,
            null,
            Color.White,
            0f,
            new Vector2(texture.Width / 2f, texture.Height / 2f),
            scale,
            SpriteEffects.None,
            0f
        );
    }
}