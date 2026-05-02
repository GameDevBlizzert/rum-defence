using System;
using Microsoft.Xna.Framework;

namespace RumDefence;

public class BossTroopAnimation : TroopAnimation
{
    public BossTroopAnimation(int frameWidth, int frameHeight, float frameDuration, int totalFrames, bool isLoop) : base(frameWidth, frameHeight, frameDuration, totalFrames, isLoop)
    {
        _walkDownLayers = [
            new SpriteLayer(0, 2, 0), // Body Down
            new SpriteLayer(8, 10, 0) // Clothes Down
        ];
        _walkUpLayers = [
            new SpriteLayer(0, 2, 1), // Body Up
            new SpriteLayer(8, 10, 1) // Clothes Up
        ];
        _walkRightLayers = [
            new SpriteLayer(0, 2, 2), // Body Right
            new SpriteLayer(8, 10, 2) // Clothes Right
        ];
        _walkLeftLayers = [
            new SpriteLayer(0, 2, 3), // Body Left
            new SpriteLayer(8, 10, 3) // Clothes Left
        ];
    }
}
public class BossTroopSwordAttackAnimation : BossTroopAnimation
{
    public BossTroopSwordAttackAnimation() : base(16, 16, 0.15f, 3, true)
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