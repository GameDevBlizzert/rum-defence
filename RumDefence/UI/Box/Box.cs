using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence.UI.Box;

public class Box : IBox
{
    public Align AlignX { get; set; } = Align.Start;
    public Align AlignY { get; set; } = Align.Start;
    public int Columns { get; set; } = 1;
    public int Rows { get; set; } = 1;
    public int Span { get; set; } = 12;
    public Color Color { get; set; } = Color.White;
    public int Gap { get; set; } = 8;
    public int Padding { get; set; } = 16;
    public int Width { get; set; }
    public int Height { get; set; }
    public Rectangle Slot { get; set; }
    protected readonly List<IBoxItem> Children = [];
    protected IBoxItem Background;
    public void Add(IBoxItem item)
    {
        Children.Add(item);
    }
    public void AddBackground(IBoxItem item)
    {
        Background = item;
    }
    public virtual Vector2 Measure()
    {
        Vector2 size = Vector2.Zero;
        Vector2 childSize;
        foreach (var child in Children)
        {
            childSize = GetChildSize(child);
            size += childSize;
        }
        size.Y += (Children.Count - 1) * Gap;
        size.X += (Children.Count - 1) * Gap;
        return size;
    }

    private Vector2 GetChildSize(IBoxItem child)
    {
        int spanX;
        Vector2 childSize = child.Measure();
        if (Span > 0)
        {
            spanX = Width / Span * child.Span;
            childSize.X = MathHelper.Max(spanX, childSize.X);
        }
        return childSize;
    }

    // sets Rectangles (location and size) for all children
    public virtual void Arrange(Rectangle rect)
    {
        IBoxItem child;
        Vector2 childSize;
        Rectangle childRect;

        // think of a grid with col as x-axis and row as y-axis
        int col, row;

        // add padding between the Box edges and content (children)
        col = rect.X + Padding;
        row = rect.Y + Padding;

        var childrenMeasured = Measure();
        // Align of content items (assuming from (0,0) Point):
        // Align.Start the first item is top left
        // Align.Center to center the contents
        if (AlignX == Align.Center)
        {
            col += (Width - (int)childrenMeasured.X) / 2;
        }
        // Align.End the last item is top right
        else if (AlignX == Align.End)
        {
            col += Width - (int)childrenMeasured.X;
        }
        for (int i = 0; i < Children.Count; i++)
        {
            child = Children[i];
            childSize = GetChildSize(child);
            // if (DirectionItems == Direction.Row)
            // {
            // y += i * Gap;
            // }
            // else if (DirectionItems == Direction.Column)
            // {
            // }

            // add gaps between children
            col += i * Gap;
            // apply location and size to children
            childRect = new Rectangle(col, row, (int)childSize.X, (int)childSize.Y);
            child.Arrange(childRect);

            col += (int)childSize.X;

        }
    }
    public void PlaceAt(int x, int y)
    {
        Slot = new Rectangle(x, y, Width, Height);
    }
    public void PlaceAt(Vector2 offset)
    {
        Slot = new Rectangle((int)offset.X, (int)offset.Y, Width, Height);
    }
    public virtual void Update(GameTime gameTime)
    {
        Arrange(Slot);
        foreach (var child in Children)
            child.Update(gameTime);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        // wip
        spriteBatch.Draw(Primitives.Pixel, Slot, Color.GreenYellow);
        if (Background != null)
        {
            Background.Draw(spriteBatch);
            // NineSlice.Draw(spriteBatch, Primitives.PanelTexture, arrangedRect, new Rectangle(0, 0, 128, 128), 20, PanelTint);
        }

        foreach (var child in Children)
            child.Draw(spriteBatch);
    }
}
