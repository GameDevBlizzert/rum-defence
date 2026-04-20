using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class IconButton : Button
{
    private FantasyPanel background;
    private Texture2D iconTexture;

    // How much of the button area the icon fills (0.0 - 1.0)
    private const float IconScale = 0.62f;

    public bool IsDisabled { get; set; }

    public IconButton(Texture2D iconTexture, Vector2 position, Vector2 size)
    {
        this.iconTexture = iconTexture;
        background = new FantasyPanel(0);

        SetBounds(new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)size.X,
            (int)size.Y
        ));
    }

    public override void SetBounds(Rectangle rect)
    {
        base.SetBounds(rect);
        if (background != null)
            background.SetBounds(rect);
    }

    protected override bool IsClickable() => !IsDisabled;

    public override void Draw(SpriteBatch spriteBatch)
    {
        int iconSize = (int)(Math.Min(bounds.Width, bounds.Height) * IconScale);
        var iconRect = new Rectangle(
            bounds.X + (bounds.Width - iconSize) / 2,
            bounds.Y + (bounds.Height - iconSize) / 2,
            iconSize,
            iconSize
        );

        if (IsDisabled)
        {
            // Disabled: plain dark icon
            var iconColor = new Color(80, 80, 80);
            spriteBatch.Draw(iconTexture, iconRect, iconColor);
        }
        else if (isSelected)
        {
            // Active/Selected: white background with border and white icon
            background.DrawCenterFill = true;
            background.Tint = Color.White;
            background.Draw(spriteBatch);
            spriteBatch.Draw(iconTexture, iconRect, Color.White);
        }
        else if (isHovering)
        {
            // Hover: show border only with bright icon
            background.DrawCenterFill = false;
            background.Tint = Color.White;
            background.Draw(spriteBatch);
            spriteBatch.Draw(iconTexture, iconRect, Color.White);
        }
        else
        {
            // Normal: plain white icon, no background or border
            spriteBatch.Draw(iconTexture, iconRect, Color.White);
        }
    }
}
