using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class MusketTower : BaseTower
{
    public MusketTower(TowerData data, Vector2 location, List<Troop> troops) : base(data, location, troops)
    {
        // Sprite sheet columns: 0=Left, 1=Down, 2=Right, 3=Up
        // Row 0: the rotating barrel layer (WithRotation applied in draw)
        // Rows 1-4: static base layers per upgrade level
        animation.AddLayerMatrix(
            [
                new(1, SpriteAction.Rotation, SpriteDirection.Left),
                new(1, SpriteAction.Rotation, SpriteDirection.Down),
                new(1, SpriteAction.Rotation, SpriteDirection.Right, SpriteEffects.FlipVertically),
                new(1, SpriteAction.Rotation, SpriteDirection.Up),
            ]
        );
        animation.AddLayerMatrix(
            [
                new(1, SpriteAction.Static, SpriteDirection.Left),
                new(1, SpriteAction.Static, SpriteDirection.Down),
                new(1, SpriteAction.Static, SpriteDirection.Right),
                new(1, SpriteAction.Static, SpriteDirection.Up),
            ]
        , 4);
    }
}
