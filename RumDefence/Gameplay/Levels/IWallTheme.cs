using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public interface IWallTheme
{
    Texture2D Wall { get; }
    Texture2D End { get; }
    Texture2D Corner { get; }
    Texture2D GetDamagedWall(int x, int y);
    Texture2D GetDamagedEnd(int x, int y);
    Texture2D GetDamagedCorner(int x, int y);
}