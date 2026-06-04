using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence.UI.Box;

public class Box : IBox
{
    public Align AlignItems { get; set; } = Align.Start;
    public Direction DirectionItems { get; set; } = Direction.Column;
    public Color Color { get; set; } = Color.White;
    public int Gap { get; set; } = 8;
    public int Padding { get; set; } = 16;
    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }
    protected Rectangle Rect;
    protected readonly List<IBoxItem> Children = [];
    protected IBoxItem Background;
    public Box Add(IBoxItem item)
    {
        Children.Add(item);
        return this;
    }
    public Box AddBackground(IBoxItem item)
    {
        Background = item;
        return this;
    }
    public virtual Vector2 Measure()
    {
        Vector2 size = Vector2.Zero;
        Vector2 childSize;
        foreach (var child in Children)
        {
            childSize = child.Measure();
            size.X = MathHelper.Max(size.X, childSize.X);
            size.Y = MathHelper.Max(size.Y, childSize.Y);
        }
        if (DirectionItems == Direction.Row)
        {
            size.Y += (Children.Count - 1) * Gap;
        }
        else if (DirectionItems == Direction.Column)
        {
            size.X += (Children.Count - 1) * Gap;
        }
        size.X += Padding;
        size.Y += Padding;
        return size;
    }
    public virtual void Arrange(Rectangle rect)
    {
        IBoxItem child;
        Vector2 childSize;
        Rectangle childRect;
        int x, y;
        x = rect.X + Padding;
        y = rect.Y + Padding;
        var childrenSize = Measure();
        if (AlignItems == Align.Center)
        {
            x = (x - (int)childrenSize.X) / 2;
        }
        else if (AlignItems == Align.End)
        {

        }
        for (int i = 0; i < Children.Count; i++)
        {
            child = Children[i];
            childSize = child.Measure();

            // if (DirectionItems == Direction.Row)
            // {
            // y += i * Gap;
            // }
            // else if (DirectionItems == Direction.Column)
            // {
            x += i * Gap;
            // }
            childRect = new Rectangle(x, y, (int)childSize.X, (int)childSize.Y);
            child.Arrange(childRect);

            x += (int)childSize.X;

        }
    }
    public void PlaceAt(Vector2 offset)
    {
        Arrange(new Rectangle(offset.ToPoint(), Measure().ToPoint()));
    }

    public virtual void Update(GameTime gameTime)
    {
        foreach (var child in Children)
            child.Update(gameTime);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Background != null)
        {
            //     NineSlice.Draw(spriteBatch, Primitives.PanelTexture, arrangedRect, new Rectangle(0, 0, 128, 128), 20, PanelTint);
        }

        foreach (var child in Children)
            child.Draw(spriteBatch);
    }
}
