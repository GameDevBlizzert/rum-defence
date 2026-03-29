using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public interface ITileTheme
{
    Texture2D GetTexture(int tile, int x, int y);
    Texture2D GetShip(string name);
    Texture2D GetRandomEnemy();
}