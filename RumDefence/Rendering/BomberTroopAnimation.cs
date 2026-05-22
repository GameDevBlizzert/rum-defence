using System;
using Microsoft.Xna.Framework;

namespace RumDefence;

public class BomberTroopAnimation : TroopAnimation
{
    public BomberTroopAnimation(int frameWidth, int frameHeight, float frameDuration, int totalFrames, bool isLoop) : base(frameWidth, frameHeight, frameDuration, totalFrames, isLoop)
    {
        _walkDownLayers = [
            new SpriteLayer(12, 14, 8), // Body Down
            new SpriteLayer(12, 14, 10), // Clothes Down
            new SpriteLayer(12, 14, 7) // Bomb Down
        ];
        _walkUpLayers = [
            new SpriteLayer(15, 17, 8), // Body Up
            new SpriteLayer(15, 17, 10), // Clothes Up
            new SpriteLayer(15, 17, 7) // Bomb Up
        ];
        _walkRightLayers = [
            new SpriteLayer(18, 20, 8), // Body Right
            new SpriteLayer(18, 20, 10), // Clothes Right
            new SpriteLayer(18, 20, 7) // Bomb Right
        ];
        _walkLeftLayers = [
            new SpriteLayer(21, 23, 8), // Body Left
            new SpriteLayer(21, 23, 10), // Clothes Left
            new SpriteLayer(21, 23, 7) // Bomb Left
        ];
    }
}
public class BomberTroopExplodeAnimation : TroopAnimation
{
    public BomberTroopExplodeAnimation() : base(16, 16, 0.15f, 3, true)
    {
        _walkDownLayers = [
            new SpriteLayer(0, 2, 12),
            new SpriteLayer(8, 10, 12),
        ];
        _walkUpLayers = [
            new SpriteLayer(0, 2, 13),
            new SpriteLayer(8, 10, 13),
        ];
        _walkRightLayers = [
            new SpriteLayer(0, 2, 14),
            new SpriteLayer(8, 10, 14),
        ];
        _walkLeftLayers = [
            new SpriteLayer(0, 2, 15),
            new SpriteLayer(8, 10, 15),
        ];
    }
}

public class BomberTroopDyingAnimation : TroopDyingAnimation
{
    private SpriteLayer[] _layers = [
        new (24, 26, 8),
        new (24, 26, 18),
    ];
}