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
        int GapBetween;
        for (int i = 0; i < Children.Count; i++)
        {
            child = Children[i];
            childSize = child.Measure();
            if (i + 1 < Children.Count)
                GapBetween = Gap;
            else
                GapBetween = 0;
            if (Direction == Direction.Column)
            {
                size.X += childSize.X + GapBetween;
                size.Y = MathHelper.Max(childSize.Y, size.Y);
            }
            else if (Direction == Direction.Row)
            {
                size.X = MathHelper.Max(childSize.X, size.X);
                size.Y += childSize.Y + GapBetween;
            }
        }
        return size;
    }
    // sets Rectangles (location and size) for all children
    public override void Arrange(Rectangle rect)
    {
        IBox child;
        Vector2 childSize;
        Rectangle childRect;

        var childrenMeasured = Measure();

        int x, y;
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

        int GapBetween;
        for (int i = 0; i < Children.Count; i++)
        {
            child = Children[i];
            childSize = child.Measure();
            if (i + 1 < Children.Count)
                GapBetween = Gap;
            else
                GapBetween = 0;
            childRect = new Rectangle(x, y, (int)childSize.X, (int)childSize.Y);
            child.Arrange(childRect);
            if (Direction == Direction.Column)
            {
                x += GapBetween;
                x += (int)childSize.X;
            }
            else if (Direction == Direction.Row)
            {
                y += GapBetween;
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
    }
    public void PlaceAt(int x = 0, int y = 0, int width = 0, int height = 0)
    {
        var childrenMeasured = Measure();
        if (width == 0)
            width = (int)childrenMeasured.X;
        if (height == 0)
            height = (int)childrenMeasured.Y;
        Slot = new(x, y, width + 2 * Padding, height + 2 * Padding);
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
