using System;
using Microsoft.Xna.Framework;

namespace RumDefence;

public class MusketAnimation : Animation
{
    private readonly SpriteLayer _barrelOuterLayer = new SpriteLayer(0, 0, 0);
    private readonly SpriteLayer _barrelInnerLayer = new SpriteLayer(0, 0, 4);
    private readonly SpriteLayer _musketLeftRightLayer = new SpriteLayer(0, 0, 2);
    private readonly SpriteLayer _musketUpDownLayer = new SpriteLayer(1, 1, 2);

    private readonly SpriteLayer _pirateRightLayer = new SpriteLayer(0, 0, 3);
    private readonly SpriteLayer _pirateDownLayer = new SpriteLayer(1, 1, 3);
    private readonly SpriteLayer _pirateLeftLayer = new SpriteLayer(2, 2, 3);
    private readonly SpriteLayer _pirateUpLayer = new SpriteLayer(3, 3, 3);

    public MusketAnimation() : base(128, 128, 0f, 1, false) { }

    public Rectangle GetBarrelOuterRectangle() =>
        _barrelOuterLayer.GetSourceRectangle(0, FrameWidth, FrameHeight);
    public Rectangle GetBarrelInnerRectangle() =>
        _barrelInnerLayer.GetSourceRectangle(0, FrameWidth, FrameHeight);
    public Rectangle GetPirateRectangle(Vector2 direction)
    {
        SpriteLayer layer;
        if (Math.Abs(direction.X) >= Math.Abs(direction.Y))
            layer = direction.X > 0 ? _pirateLeftLayer : _pirateRightLayer;
        else
            layer = direction.Y > 0 ? _pirateDownLayer : _pirateUpLayer;
        return layer.GetSourceRectangle(0, FrameWidth, FrameHeight);
    }

    public Rectangle GetMusketRectangle(Vector2 direction)
    {
        SpriteLayer layer = Math.Abs(direction.X) >= Math.Abs(direction.Y)
            ? _musketLeftRightLayer
            : _musketUpDownLayer;
        return layer.GetSourceRectangle(0, FrameWidth, FrameHeight);
    }

    public override Rectangle[] GetCurrentLayerRectangles(GameTime gameTime, Vector2 direction) =>
        new[] { GetMusketRectangle(direction) };
}
