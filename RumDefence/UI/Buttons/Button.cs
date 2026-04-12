using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace RumDefence;

public abstract class Button : UIElement
{
    protected Rectangle bounds;

    protected bool isHovering;
    protected bool wasPressed;

    public Action OnClick;

    public void SetBounds(Rectangle rect)
    {
        bounds = rect;
        Position = new Vector2(rect.X, rect.Y);
    }
    protected virtual bool IsClickable()
    {
        return true;
    }

    public override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();

        var mousePos = ScreenManager.GetMousePositionScaled();

        var mouseRect = new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1);

        isHovering = mouseRect.Intersects(bounds);

        bool isPressed = mouse.LeftButton == ButtonState.Pressed;

        if (isHovering && !isPressed && wasPressed && IsClickable())
        {
            AudioManager.Instance.PlaySound("confirmation");
            OnClick?.Invoke();
        }

        wasPressed = isPressed;
    }
}
