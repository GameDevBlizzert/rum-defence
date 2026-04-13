using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class IconButton : Button
{
    private Texture2D backgroundTexture;
    private Texture2D iconTexture;

    // How much of the button area the icon fills (0.0 - 1.0)
    private const float IconScale = 0.62f;

    public Color BaseTint { get; set; } = Color.White;

    public IconButton(Texture2D backgroundTexture, Texture2D iconTexture, Vector2 position, Vector2 size)
    {
        this.backgroundTexture = backgroundTexture;
        this.iconTexture = iconTexture;

        SetBounds(new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)size.X,
            (int)size.Y
        ));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Color bgColor = BaseTint;

        if (isSelected)
            bgColor = Color.Multiply(BaseTint, 0.55f);
        else if (isHovering)
            bgColor = Color.Multiply(BaseTint, 0.8f);

        // Background
        spriteBatch.Draw(backgroundTexture, bounds, bgColor);

        // Icon centred and scaled
        int iconSize = (int)(Math.Min(bounds.Width, bounds.Height) * IconScale);
        var iconRect = new Rectangle(
            bounds.X + (bounds.Width - iconSize) / 2,
            bounds.Y + (bounds.Height - iconSize) / 2,
            iconSize,
            iconSize
        );

        spriteBatch.Draw(iconTexture, iconRect, Color.White);
    }
}
