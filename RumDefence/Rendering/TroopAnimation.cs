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
    public TroopAnimation(int frameWidth, int frameHeight, float frameDuration)
        : base(frameWidth, frameHeight, frameDuration)
    {
        ColumnOffset = 12;
        AddLayerMatrix(
        [
            new(3, SpriteAction.Idle, SpriteDirection.Down),
            new(3, SpriteAction.Idle, SpriteDirection.Up),
            new(3, SpriteAction.Walking, SpriteDirection.Down),
            new(3, SpriteAction.Walking, SpriteDirection.Up),
            new(3, SpriteAction.Walking, SpriteDirection.Right),
            new(3, SpriteAction.Walking, SpriteDirection.Left),
            new(4, SpriteAction.Dying, SpriteDirection.Right, isLoop: false),
            new(3, SpriteAction.Attack, SpriteDirection.Down),
            new(3, SpriteAction.Attack, SpriteDirection.Up),
            new(3, SpriteAction.Attack, SpriteDirection.Right),
            new(3, SpriteAction.Attack, SpriteDirection.Left),
        ], 4);
        ActivateLayers([new(SpriteAction.Idle, SpriteDirection.Down)]);
    }
}
