using System;
using Microsoft.Xna.Framework;

namespace RumDefence;

public class TroopAnimation : Animation
{
    private SpriteLayer[] _currentActiveLayers;

    protected virtual SpriteLayer[] _walkDownLayers { get; set; }
    protected virtual SpriteLayer[] _walkUpLayers { get; set; }
    protected virtual SpriteLayer[] _walkRightLayers { get; set; }
    protected virtual SpriteLayer[] _walkLeftLayers { get; set; }
    public TroopAnimation(int frameWidth, int frameHeight, float frameDuration, int totalFrames, bool isLoop) : base(frameWidth, frameHeight, frameDuration, totalFrames, isLoop)
    {
        _currentActiveLayers = _walkDownLayers;
        _walkDownLayers = [
            new SpriteLayer(0, 2, 0), // Body Down
            new SpriteLayer(4, 6, 0), // Hat Down
        ];
        _walkUpLayers = [
            new SpriteLayer(0, 2, 1), // Body Up
            new SpriteLayer(4, 6, 1), // Hat Up
        ];
        _walkRightLayers = [
            new SpriteLayer(0, 2, 2), // Body Right
            new SpriteLayer(4, 6, 2), // Hat Right
        ];
        _walkLeftLayers = [
            new SpriteLayer(0, 2, 3), // Body Left
            new SpriteLayer(4, 6, 3), // Hat Left
        ];
    }
    public void Update(GameTime gameTime, Vector2 direction)
    {
        SetWalkDirection(direction);
        if (direction != Vector2.Zero)
        {
            ElapseTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            CurrentFrame = (int)(ElapseTime / FrameDuration) % FrameTotal;
        }
        else
        {
            ElapseTime = 0;
            CurrentFrame = 0;
        }
    }

    public override Rectangle[] GetCurrentLayerRectangles(GameTime gameTime, Vector2 direction)
    {
        Update(gameTime, direction);
        if (_currentActiveLayers == null)
            return Array.Empty<Rectangle>();

        Rectangle[] rects = new Rectangle[_currentActiveLayers.Length];

        for (int i = 0; i < _currentActiveLayers.Length; i++)
        {
            rects[i] = _currentActiveLayers[i].GetSourceRectangle(CurrentFrame, FrameWidth, FrameHeight);
        }

        return rects;
    }
    public void SetWalkDirection(Vector2 direction)
    {
        if (Math.Abs(direction.X) > Math.Abs(direction.Y))
        {
            if (direction.X > 0)
            {
                _currentActiveLayers = _walkRightLayers;
            }
            else
            {
                _currentActiveLayers = _walkLeftLayers;
            }
        }
        else
        {
            if (direction.Y > 0)
            {
                _currentActiveLayers = _walkDownLayers;
            }
            else
            {
                _currentActiveLayers = _walkUpLayers;
            }
        }
    }
}
public class TroopDyingAnimation : Animation
{
    private SpriteLayer[] _layers = [
        new (0, 3, 18),
        new (4, 7, 18),
    ];

    public bool IsFinished { get; private set; }

    public TroopDyingAnimation() : base(16, 16, 0.15f, 4, false) { }

    public override Rectangle[] GetCurrentLayerRectangles(GameTime gameTime, Vector2 direction)
    {
        if (!IsFinished)
        {
            ElapseTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            CurrentFrame = (int)(ElapseTime / FrameDuration);
            if (CurrentFrame >= FrameTotal)
            {
                CurrentFrame = FrameTotal - 1;
                IsFinished = true;
            }
        }

        Rectangle[] rects = new Rectangle[_layers.Length];
        for (int i = 0; i < _layers.Length; i++)
            rects[i] = _layers[i].GetSourceRectangle(CurrentFrame, FrameWidth, FrameHeight);
        return rects;
    }
}

public class TroopSwordAttackAnimation : TroopAnimation
{
    public TroopSwordAttackAnimation() : base(16, 16, 0.15f, 3, true)
    {
        _walkDownLayers = [
            new SpriteLayer(0, 2, 12),
            new SpriteLayer(4, 6, 12),
        ];
        _walkUpLayers = [
            new SpriteLayer(0, 2, 13),
            new SpriteLayer(4, 6, 13),
        ];
        _walkRightLayers = [
            new SpriteLayer(0, 2, 14),
            new SpriteLayer(4, 6, 14),
        ];
        _walkLeftLayers = [
            new SpriteLayer(0, 2, 15),
            new SpriteLayer(4, 6, 15),
        ];
    }
}
