using System;
using Microsoft.Xna.Framework;

namespace RumDefence;

public class MusketAnimation : Animation
{
    // Column 0, Row 0: left/right facing sprite
    private readonly SpriteLayer _leftRightLayer = new SpriteLayer(0, 0, 0);
    // Column 1, Row 0: up/down facing sprite
    private readonly SpriteLayer _upDownLayer = new SpriteLayer(1, 1, 0);

    public MusketAnimation() : base(64, 64, 0f, 1, false) { }

    public override Rectangle[] GetCurrentLayerRectangles(GameTime gameTime, Vector2 direction)
    {
        // Pick the cell whose cardinal direction is closest to current rotation.
        // When |horizontal| >= |vertical|, the tower points mostly left or right.
        SpriteLayer layer = Math.Abs(direction.X) >= Math.Abs(direction.Y)
            ? _leftRightLayer
            : _upDownLayer;

        return new[] { layer.GetSourceRectangle(0, FrameWidth, FrameHeight) };
    }
}
