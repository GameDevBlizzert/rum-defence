using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

    public void DrawLayers(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 origin,
        float scale, Color color, float rotation, float rotationOffset, float layerDepth)
    {
        Vector2 dir = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
        bool facingHorizontal = Math.Abs(dir.X) >= Math.Abs(dir.Y);
        SpriteEffects musketEffect = facingHorizontal && dir.X > 0
            ? SpriteEffects.FlipVertically
            : SpriteEffects.None;

        spriteBatch.Draw(texture, position, GetBarrelInnerRectangle(),
            color, 0f, origin, scale, SpriteEffects.None, layerDepth);

        spriteBatch.Draw(texture, position, GetPirateRectangle(dir),
            color, 0f, origin, scale, SpriteEffects.None, layerDepth + 0.02f);

        spriteBatch.Draw(texture, position, GetBarrelOuterRectangle(),
            color, 0f, origin, scale, SpriteEffects.None, layerDepth + 0.03f);

        spriteBatch.Draw(texture, position, GetMusketRectangle(dir),
            color, rotation + rotationOffset, origin, scale, musketEffect, layerDepth + 0.04f);
    }

    public override Rectangle[] GetCurrentLayerRectangles(GameTime gameTime, Vector2 direction) =>
        new[] { GetMusketRectangle(direction) };
}
