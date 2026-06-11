using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence.UI.Box;

public enum Direction { Row, Column }
public class Box : IBox
{
    public Align AlignX { get; set; } = Align.Center;
    public Align AlignY { get; set; } = Align.Center;
    public Direction Direction { get; set; } = Direction.Column;
    public int Gap { get; set; } = 2;
    public int Padding { get; set; } = 4;
    public override Vector2 Measure()
    {
        Vector2 size = Vector2.Zero;
        Vector2 childSize;
        IBox child;
        for (int i = Children.Count; i > 0; i--)
        {
            child = Children[^i];
            childSize = child.Measure();
            if (Direction == Direction.Column)
            {
                size.X += childSize.X + GetGap(i);
                size.Y = MathHelper.Max(childSize.Y, size.Y);
            }
            else if (Direction == Direction.Row)
            {
                size.X = MathHelper.Max(childSize.X, size.X);
                size.Y += childSize.Y + GetGap(i);
            }
        }
        size += new Vector2(2 * Padding);
        return size;
    }
    private int GetGap(int index)
    {
        return (1 % index + 0) * Gap;
    }
    private int GetGap(int index, Align align, int extraSpace)
    {
        // children verdelen 51% aan het begin en 49% aan het eind 
        if (align == Align.Between && Children.Count > 1 && index == Children.Count / 2 + 1)
            return Gap + extraSpace;
        return GetGap(index);
    }
    // sets Rectangles (location and size) for all children
    public override void Arrange(Rectangle rect)
    {
        int x, y;
        // backup. ingeval iemand 0 om 0 geeft.
        var childrenMeasured = Measure();
        if (rect.Width == 0 || rect.Width < childrenMeasured.X)
            rect.Width = (int)childrenMeasured.X + 2 * Padding;
        if (rect.Height == 0 || rect.Height < childrenMeasured.Y)
            rect.Height = (int)childrenMeasured.Y + 2 * Padding;
        x = rect.X;
        y = rect.Y;
        var Width = rect.Width - 2 * Padding;
        var Height = rect.Height - 2 * Padding;

        // padding tussen the Box edges en content (children)
        x += Padding;
        y += Padding;

        var extraX = MathHelper.Max(0, Width - (int)childrenMeasured.X);
        var extraY = MathHelper.Max(0, Height - (int)childrenMeasured.Y);

        // Align content between box and children
        if (AlignX == Align.Center)
            x += (Width - (int)childrenMeasured.X) / 2;
        else if (AlignX == Align.End)
            x += Width - (int)childrenMeasured.X;

        if (AlignY == Align.Center)
            y += (Height - (int)childrenMeasured.Y) / 2;
        else if (AlignY == Align.End)
            y += Height - (int)childrenMeasured.Y;

        IBox child;
        Vector2 childSize;
        Rectangle childRect;
        int childX;
        int childY;
        for (int i = Children.Count; i > 0; i--)
        {
            childX = x;
            childY = y;
            child = Children[^i];
            childSize = child.Measure();
            // Align Content and Direction between children and child 
            if (Direction == Direction.Row)
                if (AlignX == Align.Center)
                    childX += ((int)childrenMeasured.X - (int)childSize.X) / 2;
                else if (AlignX == Align.End)
                    childX += (int)childrenMeasured.X - (int)childSize.X;

            if (Direction == Direction.Column)
                if (AlignY == Align.Center)
                    childY += ((int)childrenMeasured.Y - (int)childSize.Y) / 2;
                else if (AlignY == Align.End)
                    childY += (int)childrenMeasured.Y - (int)childSize.Y;

            childRect = new Rectangle(childX, childY, (int)childSize.X, (int)childSize.Y);
            child.Arrange(childRect);

            // Direction stacking of Gap and child
            if (Direction == Direction.Column)
            {
                x += GetGap(i, AlignX, extraX);
                x += (int)childSize.X;
            }
            else if (Direction == Direction.Row)
            {
                y += GetGap(i, AlignY, extraY);
                y += (int)childSize.Y;
            }
        }
        base.Arrange(rect);
    }
    public virtual void PlaceAt(int x = 0, int y = 0, int width = 0, int height = 0)
    {
        Arrange(new(x, y, width, height));
    }
    public override void UpdateBox(GameTime gameTime)
    {
        Arrange(Slot);
        foreach (var child in Children)
            child.Update(gameTime);
    }
    public override void DrawBox(SpriteBatch spriteBatch)
    {
        foreach (var child in Children)
            child.Draw(spriteBatch);
    }
}
