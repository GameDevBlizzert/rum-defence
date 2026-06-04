using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence.UI.Box;

public enum Align { Start, Center, Between, End }
public enum Direction { Row, Column }

public interface IBox : IBoxItem
{
    Align AlignItems { get; set; }
    Direction DirectionItems { get; set; }
    int Gap { get; set; }
    int Padding { get; set; }
    int MaxWidth { get; set; }
    int MaxHeight { get; set; }

}
