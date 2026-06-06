using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence.UI.Box;

// Align the contents
public enum Align { Start, Center, Between, End }
public abstract class IBox
{
    public Align AlignX { get; set; } = Align.Center;
    public Align AlignY { get; set; } = Align.Center;
    public Color Color { get; set; } = Color.White;
    public Rectangle Slot { get; set; }
    private bool isActive = false;
    protected bool IsActive { get => isActive; }
    public virtual Vector2 Measure()
    {
        return Vector2.Zero;
    }
    public virtual void Arrange(Rectangle rect)
    {
        Slot = rect;
    }
    public void Activate()
    {
        isActive = true;
    }
    public void Deactivate()
    {
        isActive = false;
    }
    public virtual void Update(GameTime gameTime) { }
    public virtual void DrawBox(SpriteBatch spriteBatch) { }
    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;
        DrawBox(spriteBatch);
    }
}
