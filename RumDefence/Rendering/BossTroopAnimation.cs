using Microsoft.Xna.Framework;

namespace RumDefence;

public class BossTroopAnimation : TroopAnimation
{
    public BossTroopAnimation(int frameWidth, int frameHeight, float frameDuration, bool isLoop)
        : base(frameWidth, frameHeight, frameDuration, isLoop)
    {
        ClearLayers();
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 0, 2, 0));    // Body Down
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 8, 10, 0));   // Clothes Down
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 0, 2, 1));      // Body Up
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 8, 10, 1));     // Clothes Up
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 0, 2, 2));   // Body Right
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 8, 10, 2));  // Clothes Right
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 0, 2, 3));    // Body Left
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 8, 10, 3));   // Clothes Left
        UpdateActiveLayers(SpriteType.Down);
    }
}

public class BossTroopSwordAttackAnimation : TroopSwordAttackAnimation
{
    public BossTroopSwordAttackAnimation() : base()
    {
        ClearLayers();
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 0, 2, 12));
        AddSpriteLayer(new SpriteLayer(SpriteType.Down, 8, 10, 12));
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 0, 2, 13));
        AddSpriteLayer(new SpriteLayer(SpriteType.Up, 8, 10, 13));
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 0, 2, 14));
        AddSpriteLayer(new SpriteLayer(SpriteType.Right, 8, 10, 14));
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 0, 2, 15));
        AddSpriteLayer(new SpriteLayer(SpriteType.Left, 8, 10, 15));
        UpdateActiveLayers(SpriteType.Down);
    }
}
