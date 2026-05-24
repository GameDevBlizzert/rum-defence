using System;
using Microsoft.Xna.Framework;

namespace RumDefence;

/*
Troop Sprite row description:
row_index   layer
0           effect
1           tool
2           clothes
3           body
*/

public class TroopAnimation : Animation
{
    public TroopAnimation(int frameWidth, int frameHeight, float frameDuration, bool isLoop)
        : base(frameWidth, frameHeight, frameDuration, isLoop)
    {
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 12, 14, 3));
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 12, 14, 2));
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 12, 14, 1));
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 12, 14, 0));
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 15, 17, 3));
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 15, 17, 2));
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 15, 17, 1));
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 15, 17, 0));
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 18, 20, 3));
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 18, 20, 2));
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 18, 20, 1));
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 18, 20, 0));
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 21, 23, 3));
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 21, 23, 2));
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 21, 23, 1));
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 21, 23, 0));
        UpdateActiveLayers(SpriteType.Down);
    }

    public Rectangle[] GetCurrentLayerRectangles(GameTime gameTime, Vector2 direction)
    {
        ActivateLayerType(direction);
        return base.GetCurrentLayerRectangles(gameTime);
    }

    public void SetWalkDirection(Vector2 direction)
    {
        ActivateLayerType(direction);
    }

    public void ActivateLayerType(Vector2 direction)
    {
        if (Math.Abs(direction.X) > Math.Abs(direction.Y))
        {
            UpdateActiveLayers(direction.X > 0 ? SpriteType.Right : SpriteType.Left);
        }
        else
        {
            UpdateActiveLayers(direction.Y > 0 ? SpriteType.Down : SpriteType.Up);
        }
    }
}

public class TroopDyingAnimation : Animation
{
    public TroopDyingAnimation() : base(16, 16, 0.15f, false)
    {
        AddSpriteLayer(new SpriteLayer(SpriteType.Dying, 24, 27, 3));
        AddSpriteLayer(new SpriteLayer(SpriteType.Dying, 24, 27, 2));
        AddSpriteLayer(new SpriteLayer(SpriteType.Dying, 24, 27, 1));
        AddSpriteLayer(new SpriteLayer(SpriteType.Dying, 24, 27, 0));
        UpdateActiveLayers(SpriteType.Dying);
    }
}

public class TroopSwordAttackAnimation : TroopAnimation
{
    public TroopSwordAttackAnimation() : base(16, 16, 0.15f, true)
    {
        ClearLayers();
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 28, 30, 3));
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 28, 30, 2));
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 28, 30, 1));
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 28, 30, 0));
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 31, 33, 3));
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 31, 33, 2));
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 31, 33, 1));
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 31, 33, 0));
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 34, 36, 3));
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 34, 36, 2));
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 34, 36, 1));
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 34, 36, 0));
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 37, 39, 3));
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 37, 39, 2));
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 37, 39, 1));
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 37, 39, 0));
        UpdateActiveLayers(SpriteType.Down);
    }
}
