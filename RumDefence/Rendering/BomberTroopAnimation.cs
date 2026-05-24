using Microsoft.Xna.Framework;

namespace RumDefence;

public class BomberTroopAnimation : TroopAnimation
{
    public BomberTroopAnimation(int frameWidth, int frameHeight, float frameDuration, bool isLoop)
        : base(frameWidth, frameHeight, frameDuration, isLoop)
    {
        ClearLayers();
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 14, 16, 9));
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 14, 16, 7));
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 14, 16, 6));
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 17, 19, 9));
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 17, 19, 7));
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 17, 19, 6));
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 18, 20, 9));
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 18, 20, 7));
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 18, 20, 6));
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 21, 23, 9));
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 21, 23, 7));
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 21, 23, 6));
        UpdateActiveLayers(SpriteType.Down);
    }
}

public class BomberTroopDyingAnimation : TroopDyingAnimation
{
    public BomberTroopDyingAnimation() : base()
    {
        ClearLayers();
        AddSpriteLayer(new SpriteLayer(SpriteType.Dying, 24, 27, 9));
        AddSpriteLayer(new SpriteLayer(SpriteType.Dying, 24, 27, 7));
        AddSpriteLayer(new SpriteLayer(SpriteType.Dying, 24, 27, 6));
        UpdateActiveLayers(SpriteType.Dying);
    }
}
