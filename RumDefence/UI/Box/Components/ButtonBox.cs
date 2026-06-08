using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace RumDefence.UI.Box;

public class ButtonBox : Box
{
    public Action OnClick;
    public Color BaseTint { get; set; } = Color.White;
    public bool IsDisabled { get; set; }
    public bool IsSelected { get; set; }
    public bool IsHovered { get; private set; }
    public TextItem Label { get; }
    public ImageBox Icon { get; set; }

    // Fixed size so a parent Box can lay this button out alongside its siblings.
    public Vector2 Size { get; set; }

    private bool isHovering;
    private bool isPressed;
    private bool wasPressed;

    public ButtonBox() { }

    public ButtonBox(Texture2D backgroundTexture, string text, float textScale = 1f, Vector2? size = null)
    {
        AddBackground(new ImageBox(backgroundTexture));
        Label = new TextItem(text, textScale);
        Add(Label);
        Size = size ?? Vector2.Zero;
    }

    // Reports a fixed size to the parent layout, regardless of the label's measured text size —
    // the background/label should fill the button's own bounds, not size the button to fit them.
    public override Vector2 Measure() => Size;

    public override void Arrange(Rectangle rect)
    {
        if (Size != Vector2.Zero)
            rect = new Rectangle(rect.X, rect.Y, (int)Size.X, (int)Size.Y);

        Slot = rect;
        Background?.Arrange(rect);

        var contentRect = new Rectangle(
            rect.X + Padding,
            rect.Y + Padding,
            rect.Width - 2 * Padding,
            rect.Height - 2 * Padding
        );

        foreach (var child in Children)
        {
            var childSize = child.Measure();
            int childX = contentRect.X + (contentRect.Width - (int)childSize.X) / 2;
            int childY = contentRect.Y + (contentRect.Height - (int)childSize.Y) / 2;
            child.Arrange(new Rectangle(childX, childY, (int)childSize.X, (int)childSize.Y));
        }
    }

    protected virtual bool IsClickable() => true;

    public override void UpdateBox(GameTime gameTime)
    {
        base.UpdateBox(gameTime);

        var mousePos = ScreenManager.GetMousePositionScaled();
        var mouseRect = new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1);
        isHovering = mouseRect.Intersects(Slot);
        IsHovered = isHovering;

        var mouse = Mouse.GetState();
        bool mousePressed = mouse.LeftButton == ButtonState.Pressed;
        isPressed = isHovering && mousePressed;

        if (isHovering && !mousePressed && wasPressed && !IsDisabled && IsClickable())
        {
            AudioManager.Instance.PlaySound("confirmation");
            OnClick?.Invoke();
        }

        wasPressed = mousePressed;

        ApplyTint();
    }

    private void ApplyTint()
    {
        Color tint, textColor;

        if (IsDisabled)
        {
            tint = new Color(80, 80, 80);
            textColor = Color.DarkGray;
        }
        else if (isPressed)
        {
            tint = Color.Lerp(BaseTint, Color.Black, 0.9f);
            textColor = Color.White;
        }
        else if (IsSelected)
        {
            tint = Color.Lerp(BaseTint, new Color(70, 70, 70), 0.7f);
            textColor = Primitives.FontColor;
        }
        else if (isHovering)
        {
            tint = Color.Lerp(BaseTint, new Color(40, 40, 40), 0.25f);
            textColor = Primitives.FontColor;
        }
        else
        {
            tint = BaseTint;
            textColor = Primitives.FontColor;
        }

        if (Background != null)
            Background.Color = tint;

        if (Icon != null)
            Icon.Color = IsDisabled ? new Color(80, 80, 80) : Color.White;

        foreach (var child in Children)
            if (child is TextItem textItem)
                textItem.Color = textColor;
    }
}
