using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence.UI.Box;

// Align the contents
public enum Align { Start, Center, Between, End }
public interface IBox
{
    Align AlignX { get; set; }
    Align AlignY { get; set; }
    Rectangle Slot { get; set; }
    // Measure the contents
    Vector2 Measure();
    void Update(GameTime gameTime);
    void Arrange(Rectangle rect);
    void Draw(SpriteBatch spriteBatch);
}
public abstract class IBoxItem : IBox
{
    public Align AlignX { get; set; } = Align.Center;
    public Align AlignY { get; set; } = Align.Center;
    public Color Color { get; set; } = Color.White;
    public Rectangle Slot { get; set; }
    public virtual Vector2 Measure()
    {
        return Vector2.Zero;
    }
    public virtual void Arrange(Rectangle rect)
    {
        Slot = rect;
    }
    public virtual void Update(GameTime gameTime) { }
    public virtual void Draw(SpriteBatch spriteBatch) { }
}
