using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence.UI.Box;

// A ButtonBox that centers an icon (optionally beside a cost label) instead of a text label.
public class IconButtonBox : ButtonBox
{
    private const float IconScale = 0.62f;

    public Texture2D IconTexture { get; set; }
    public string CostLabel { get; set; }

    public IconButtonBox(Texture2D backgroundTexture, Texture2D iconTexture, Rectangle? backgroundSourceRect = null)
    {
        AddBackground(new ImageBox(backgroundTexture, backgroundSourceRect));
        IconTexture = iconTexture;
    }

    public override void DrawBox(SpriteBatch spriteBatch)
    {
        base.DrawBox(spriteBatch);

        var iconColor = IsDisabled ? new Color(80, 80, 80) : Color.White;
        var labelColor = IsDisabled ? Color.DarkGray : Primitives.FontColor;

        if (!string.IsNullOrEmpty(CostLabel))
        {
            var textSize = Primitives.Font.MeasureString(CostLabel);
            const int gap = 4;
            int iconSize = (int)(Math.Min(Slot.Width, Slot.Height) * IconScale);
            float totalWidth = iconSize + gap + textSize.X;
            float startX = Slot.X + (Slot.Width - totalWidth) / 2f;
            float centerY = Slot.Y + Slot.Height / 2f;

            var iconRect = new Rectangle(
                (int)startX,
                (int)(centerY - iconSize / 2f),
                iconSize,
                iconSize
            );
            spriteBatch.Draw(IconTexture, iconRect, iconColor);

            spriteBatch.DrawString(Primitives.Font, CostLabel,
                new Vector2(startX + iconSize + gap, centerY - textSize.Y / 2f),
                labelColor);
        }
        else
        {
            int iconSize = (int)(Math.Min(Slot.Width, Slot.Height) * IconScale);
            var iconRect = new Rectangle(
                Slot.X + (Slot.Width - iconSize) / 2,
                Slot.Y + (Slot.Height - iconSize) / 2,
                iconSize,
                iconSize
            );
            spriteBatch.Draw(IconTexture, iconRect, iconColor);
        }
    }
}
