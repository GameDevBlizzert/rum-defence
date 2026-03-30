using System;
using Microsoft.Xna.Framework;
using Rum_Defence.Rendering;

public class TroopAnimation : Animation
{
    private SpriteLayer[] _currentActiveLayers;

    private readonly SpriteLayer[] _walkDownLayers = {
        new SpriteLayer(0, 2, 0), // Body Down
        new SpriteLayer(4, 6, 0), // Hat Down
        new SpriteLayer(8, 10, 0) // Clothes Down
    };
    private readonly SpriteLayer[] _walkUpLayers = {
        new SpriteLayer(0, 2, 1), // Body Up
        new SpriteLayer(4, 6, 1), // Hat Up
        new SpriteLayer(8, 10, 1) // Clothes Up
    };
    private readonly SpriteLayer[] _walkRightLayers = {
        new SpriteLayer(0, 2, 2), // Body Right
        new SpriteLayer(4, 6, 2), // Hat Right
        new SpriteLayer(8, 10, 2) // Clothes Right
    };
    private readonly SpriteLayer[] _walkLeftLayers = {
        new SpriteLayer(0, 2, 3), // Body Left
        new SpriteLayer(4, 6, 3), // Hat Left
        new SpriteLayer(8, 10, 3) // Clothes Left
    };

    public TroopAnimation(int frameWidth, int frameHeight, float frameDuration, int totalFrames, bool isLoop) : base(frameWidth, frameHeight, frameDuration, totalFrames, isLoop)
    {
        _currentActiveLayers = _walkDownLayers;
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
    private void SetWalkDirection(Vector2 direction)
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