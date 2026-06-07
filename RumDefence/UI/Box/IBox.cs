using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
    protected IBox Background;
    protected bool IsActive { get => isActive; }
    public void AddBackground(IBox item)
    {
        Background = item;
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
    public void Activate()
    {
        isActive = true;
        Background?.Activate();
    }
    public void Deactivate()
    {
        isActive = false;
        Background?.Deactivate();
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
        if (!IsActive) return;
        Background?.Draw(spriteBatch);
        DrawBox(spriteBatch);
    }
}
