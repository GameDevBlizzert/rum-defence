using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public interface IWallTheme
{
    Texture2D Single { get; }
    Texture2D Wall { get; }
    Texture2D End { get; }
    Texture2D Corner { get; }
    Texture2D Twall { get; }
    Texture2D Xwall { get; }
    Texture2D Diagonal { get; }
    Texture2D DiagonalEnd { get; }
    Texture2D DiagonalFill { get; }
    Texture2D GetDamagedWall(int x, int y);
    Texture2D GetDamagedEnd(int x, int y);
    Texture2D GetDamagedCorner(int x, int y);
    Texture2D GetDamagedTwall(int x, int y);
    Texture2D GetDamagedXwall(int x, int y);
}
