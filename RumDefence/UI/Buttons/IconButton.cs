using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class IconButton : Button
{
    private Texture2D backgroundTexture;
    private Texture2D iconTexture;

    private const float IconScale = 0.62f;

    public Color BaseTint { get; set; } = Color.White;
    public bool IsDisabled { get; set; }
    public string CostLabel { get; set; }
    public Rectangle? BackgroundSourceRect { get; set; }
    public Texture2D IconTexture
    {
        get => iconTexture;
        set => iconTexture = value;
    }

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

    protected override bool IsClickable() => !IsDisabled;

    public override void Draw(SpriteBatch spriteBatch)
    {
        Color bgColor;
        bool useDarkLabel = false;

        if (IsDisabled)
            bgColor = new Color(60, 60, 60);
        else if (isPressed)
            bgColor = Color.Lerp(BaseTint, Color.Black, 0.8f);
        else if (isSelected)
            bgColor = Color.Lerp(BaseTint, new Color(40, 40, 40), 0.45f);
        else if (isHovering)
            bgColor = Color.Lerp(BaseTint, Color.White, 0.8f);
        else
            bgColor = BaseTint;

        if (!IsDisabled && isHovering && !isPressed)
            useDarkLabel = true;

        NineSlice.Draw(spriteBatch, backgroundTexture, bounds, BackgroundSourceRect, 20, bgColor);

        var iconColor = IsDisabled ? new Color(80, 80, 80) : Color.White;

        if (CostLabel != null)
        {
            var textSize = Primitives.Font.MeasureString(CostLabel);
            const int gap = 4;
            int iconSize = (int)(Math.Min(bounds.Width, bounds.Height) * IconScale);
            float totalWidth = iconSize + gap + textSize.X;
            float startX = bounds.X + (bounds.Width - totalWidth) / 2f;
            float centerY = bounds.Y + bounds.Height / 2f;

            var iconRect = new Rectangle(
                (int)startX,
                (int)(centerY - iconSize / 2f),
                iconSize,
                iconSize
            );
            spriteBatch.Draw(iconTexture, iconRect, iconColor);

            var labelColor = IsDisabled
                ? new Color(120, 120, 120)
                : useDarkLabel
                    ? Color.Black
                    : Primitives.FontColor;
            spriteBatch.DrawString(Primitives.Font, CostLabel,
                new Vector2(startX + iconSize + gap, centerY - textSize.Y / 2f),
                labelColor);
        }
        else
        {
            int iconSize = (int)(Math.Min(bounds.Width, bounds.Height) * IconScale);
            var iconRect = new Rectangle(
                bounds.X + (bounds.Width - iconSize) / 2,
                bounds.Y + (bounds.Height - iconSize) / 2,
                iconSize,
                iconSize
            );
            spriteBatch.Draw(iconTexture, iconRect, iconColor);
        }
    }
}
