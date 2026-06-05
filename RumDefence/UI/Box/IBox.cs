using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence.UI.Box;

public enum Align { Start, Center, Between, End }
public enum Direction { Row, Column }

public interface IBox : IBoxItem
{
    Align AlignX { get; set; }
    Align AlignY { get; set; }
    int Columns { get; set; }
    int Rows { get; set; }
    int Gap { get; set; }
    int Padding { get; set; }
    int Width { get; set; }
    int Height { get; set; }

}
