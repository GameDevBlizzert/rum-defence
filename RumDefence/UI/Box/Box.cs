using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence.UI.Box;

public enum Direction { Row, Column }
public class Box : IBox
{
    public Direction Direction { get; set; } = Direction.Column;
    public int Gap { get; set; } = 8;
    public int Padding { get; set; } = 16;
    protected readonly List<IBox> Children = [];
    protected IBox Background;
    public void Add(IBox item)
    {
        Children.Add(item);
    }
    public void AddBackground(IBox item)
    {
        Background = item;
    }
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
        return size;
    }
    // sets Rectangles (location and size) for all children
    private int GetGap(int index)
    {
        return (1 % index + 0) * Gap;
    }
    public override void Arrange(Rectangle rect)
    {
        int x, y;
        var childrenMeasured = Measure();
        if (rect.Width == 0 || rect.Width < childrenMeasured.X)
            rect.Width = (int)childrenMeasured.X;
        if (rect.Height == 0 || rect.Height < childrenMeasured.Y)
            rect.Height = (int)childrenMeasured.Y;
        x = rect.X;
        y = rect.Y;
        var Width = rect.Width - 2 * Padding;
        var Height = rect.Height - 2 * Padding;

        // add padding between the Box edges and content (children)
        x += Padding;
        y += Padding;

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
            if (Direction == Direction.Column)
            {
                x += GetGap(i);
                x += (int)childSize.X;
            }
            else if (Direction == Direction.Row)
            {
                y += GetGap(i);
                y += (int)childSize.Y;
            }
            if (IsActive)
                child.Activate();
            else
                child.Deactivate();
        }
        if (Background != null)
        {
            Background.Arrange(rect);
            if (IsActive)
                Background.Activate();
            else
                Background.Deactivate();
        }
        Slot = rect;
    }
    public void PlaceAt(int x = 0, int y = 0, int width = 0, int height = 0)
    {
        Arrange(new(x, y, width, height));
        Activate();
    }
    public override void Update(GameTime gameTime)
    {
        Arrange(Slot);
        Background?.Update(gameTime);
        foreach (var child in Children)
            child.Update(gameTime);
    }
    public override void DrawBox(SpriteBatch spriteBatch)
    {
        // wip
        Background?.Draw(spriteBatch);
        // NineSlice.Draw(spriteBatch, Primitives.PanelTexture, arrangedRect, new Rectangle(0, 0, 128, 128), 20, PanelTint);

        foreach (var child in Children)
            child.Draw(spriteBatch);
    }
}
