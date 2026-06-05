using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence.UI.Box;

// Align the contents
public enum Align { Start, Center, Between, End }
public enum Direction { Row, Column }


public interface IBoxItem
{
    Align AlignX { get; set; }
    Align AlignY { get; set; }
    int Span { get; set; }
    Rectangle Slot { get; set; }
    Vector2 Measure();
    void Update(GameTime gameTime);
    void Arrange(Rectangle rect);
    void Draw(SpriteBatch spriteBatch);
}

public interface IBox : IBoxItem
{
    int Columns { get; set; }
    int Rows { get; set; }
    int Gap { get; set; }
    int Padding { get; set; }
    int Width { get; set; }
    int Height { get; set; }

}

public abstract class BoxItem : IBoxItem
{
    public Align AlignX { get; set; } = Align.Center;
    public Align AlignY { get; set; } = Align.Center;
    public int Span { get; set; } = 1;
    public Color Color { get; set; } = Color.White;
    public Rectangle Slot { get; set; }
    // Measure the contents
    public virtual Vector2 Measure()
    {
        return Vector2.Zero;
    }
    public virtual void Arrange(Rectangle rect) => Slot = rect;
    public virtual void Update(GameTime gameTime) { }
    public virtual void Draw(SpriteBatch spriteBatch)
    {

    }
}
