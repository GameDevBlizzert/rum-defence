using System;
using Microsoft.Xna.Framework;

namespace RumDefence;

public class BomberTroopAnimation : TroopAnimation
{
    public BomberTroopAnimation(int frameWidth, int frameHeight, float frameDuration, int totalFrames, bool isLoop) : base(frameWidth, frameHeight, frameDuration, totalFrames, isLoop)
    {
        _walkDownLayers = [
            new SpriteLayer(12, 14, 9), // Body Down
            new SpriteLayer(12, 14, 7), // Clothes Down
            new SpriteLayer(12, 14, 6) // Bomb Down
        ];
        _walkUpLayers = [
            new SpriteLayer(15, 17, 9), // Body Up
            new SpriteLayer(15, 17, 7), // Clothes Up
            new SpriteLayer(15, 17, 6) // Bomb Up
        ];
        _walkRightLayers = [
            new SpriteLayer(18, 20, 9), // Body Right
            new SpriteLayer(18, 20, 7), // Clothes Right
            new SpriteLayer(18, 20, 6) // Bomb Right
        ];
        _walkLeftLayers = [
            new SpriteLayer(21, 23, 9), // Body Left
            new SpriteLayer(21, 23, 7), // Clothes Left
            new SpriteLayer(21, 23, 6) // Bomb Left
        ];
    }
}

public class BomberTroopDyingAnimation : TroopDyingAnimation
{
    protected new readonly SpriteLayer[] _layers = [
        new (24, 26, 9),
        new (24, 26, 7),
        new (24, 26, 6),
    ];
}