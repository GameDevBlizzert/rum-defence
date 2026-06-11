using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence.UI.Box;

public class KeyBindBox : ButtonBox
{
    public string ActionId { get; }
    private bool IsRebinding { get; set; }
    private readonly TextItem nameLabel;
    private readonly TextItem keyLabel;
    public KeyBindBox(string actionId, string actionName)
    {
        ActionId = actionId;
        // this.anyRebinding = anyRebinding;
        nameLabel = new TextItem(actionName, 0.75f);
        keyLabel = new TextItem("", 0.75f, new Color(160, 210, 255));
    }
    public override void Arrange(Rectangle rect)
    {
        base.Arrange(rect);

        nameLabel?.Arrange(new(
            rect.X + Padding,
            rect.Y + Padding + (rect.Height - (int)nameLabel.Measure().Y) / 2,
            rect.Width - 2 * Padding,
            rect.Height - 2 * Padding
        ));
        keyLabel?.Arrange(new(
            rect.X - Padding + rect.Width - (int)keyLabel.Measure().X - 20,
            rect.Y + Padding + (rect.Height - (int)keyLabel.Measure().Y) / 2,
            rect.Width - 2 * Padding,
            rect.Height - 2 * Padding
        ));
    }
    public override void UpdateBox(GameTime gameTime)
    {
        if (InputManager.Instance.IsLeftClick())
        {
            if (MouseHover())
                IsRebinding = true;
            else
                IsRebinding = false;
        }

        keyLabel.Text = IsRebinding
            ? "Press any key..."
            : InputManager.GetKeyDisplayName(InputManager.Instance.GetBinding(ActionId));

        nameLabel.Color = IsRebinding ? new Color(255, 200, 80) : Primitives.FontColor;
        keyLabel.Color = IsRebinding ? new Color(255, 200, 80) : new Color(160, 210, 255);
        base.UpdateBox(gameTime);
    }
    public override void DrawBox(SpriteBatch spriteBatch)
    {
        bool hovered = IsHovered && !IsRebinding;

        Color bg = IsRebinding
            ? new Color(80, 60, 20) * 0.9f
            : hovered
                ? new Color(255, 255, 255) * 0.12f
                : new Color(255, 255, 255) * 0.05f;

        Color border = IsRebinding
            ? new Color(255, 200, 80)
            : new Color(255, 255, 255) * 0.2f;

        spriteBatch.Draw(Primitives.Pixel, new Rectangle(Slot.X + 1, Slot.Y + 1, Slot.Width - 1, Slot.Height - 1), border);
        spriteBatch.Draw(Primitives.Pixel, Slot, bg);
        // spriteBatch.Draw(Primitives.Pixel, new Rectangle(Slot.X, Slot.Y, Slot.Width, 1), border);
        // spriteBatch.Draw(Primitives.Pixel, new Rectangle(Slot.X, Slot.Bottom - 1, Slot.Width, 1), border);
        // spriteBatch.Draw(Primitives.Pixel, new Rectangle(Slot.X, Slot.Y, 1, Slot.Height), border);
        // spriteBatch.Draw(Primitives.Pixel, new Rectangle(Slot.Right - 1, Slot.Y, 1, Slot.Height), border);

        nameLabel.Draw(spriteBatch);
        keyLabel.Draw(spriteBatch);
    }
    // protected override bool IsClickable() => !IsRebinding && !anyRebinding();
}
