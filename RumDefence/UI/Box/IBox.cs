using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence.UI.Box;

// Align the contents
public enum Align { Start, Center, Between, End }
public abstract class IBox
{
    public Color Color { get; set; } = Color.White;
    public Rectangle Slot { get; set; }
    protected IBox Background;
    protected readonly List<IBox> Children = [];
    public void AddBackground(IBox item)
    {
        Background = item;
    }
    public void Add(IBox item)
    {
        Children.Add(item);
    }
    public virtual Vector2 Measure()
    {
        return Vector2.Zero;
    }
    public virtual void Arrange(Rectangle rect)
    {
        Slot = rect;
        Background?.Arrange(rect);
    }
    public virtual void UpdateBox(GameTime gameTime) { }
    public void Update(GameTime gameTime)
    {
        Background?.Update(gameTime);
        UpdateBox(gameTime);
    }
    public virtual void DrawBox(SpriteBatch spriteBatch) { }
    public void Draw(SpriteBatch spriteBatch)
    {
        Background?.Draw(spriteBatch);
        DrawBox(spriteBatch);
    }
}
