using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence.UI.Box;

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