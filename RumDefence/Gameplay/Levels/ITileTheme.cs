using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RumDefence;

public interface ITileTheme
{
    Texture2D GetTexture(int tile, int x, int y);
    Texture2D GetShip(string name);
    Texture2D GetRandomEnemy();
    float GetDecorationDensity();
    Ship.Data GetShipData(string type);
    (Texture2D, string) GetRandomDecoration(Random rng, int x, int y);
}