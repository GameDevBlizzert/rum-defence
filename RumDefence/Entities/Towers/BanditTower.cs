using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class BanditTower : BaseTower
{
    public BanditTower(TowerData data, Vector2 location, List<Troop> troops) : base(data, location, troops)
    {
        // Sprite sheet columns: 0=Left, 1=Down, 2=Right, 3=Up
        animation.AddLayerMatrix(
            [
                new(2, SpriteAction.Static, SpriteDirection.Right),
                new(2, SpriteAction.Static, SpriteDirection.Down),
                new(2, SpriteAction.Static, SpriteDirection.Left),
                new(2, SpriteAction.Static, SpriteDirection.Up),
            ]
        , 5);
    }
}
