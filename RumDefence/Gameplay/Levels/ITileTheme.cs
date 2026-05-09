using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence;

public interface ITileTheme
{
    Texture2D GetTexture(int tile, int x, int y);
    Texture2D GetShip(string name);
    Texture2D GetRandomEnemy();
    List<Texture2D> GetDecorations();
    float GetDecorationDensity();
    List<Texture2D> GetRocks();
    List<Texture2D> GetTrees();
    List<Texture2D> GetBushes();
}