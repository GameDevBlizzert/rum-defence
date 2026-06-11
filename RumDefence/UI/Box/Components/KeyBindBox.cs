using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RumDefence.UI.Box;

public class KeyBindBox : ButtonBox
{
    public string ActionId { get; }
    private bool IsRebinding { get; set; }
    private readonly TextItem nameLabel;
    private readonly TextItem keyLabel;
    private readonly int borderSize = 8;
    public KeyBindBox(string actionId, string actionName, float scale = 1f)
    {
        ActionId = actionId;
        nameLabel = new TextItem(actionName, scale);
        keyLabel = new TextItem("", scale, new Color(160, 210, 255));
    }

    public override void Arrange(Rectangle rect)
    {
        base.Arrange(rect);

        nameLabel?.Arrange(new(
            rect.X + Padding + borderSize,
            rect.Y + (rect.Height - (int)nameLabel.Measure().Y) / 2,
            rect.Width - 2 * Padding + borderSize,
            rect.Height - 2 * Padding + borderSize
        ));
        keyLabel?.Arrange(new(
            rect.X - Padding - borderSize + rect.Width - (int)keyLabel.Measure().X,
            rect.Y + (rect.Height - (int)keyLabel.Measure().Y) / 2,
            rect.Width - 2 * Padding + borderSize,
            rect.Height - 2 * Padding + borderSize
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
        if (IsRebinding)
        {
            if (InputManager.Instance.IsAnyKeyJustPressed(out Keys pressedKey))
            {
                if (pressedKey == Keys.Escape)
                    IsRebinding = false;
                else
                {
                    InputManager.Instance.SetBinding(ActionId, pressedKey);
                    IsRebinding = false;
                }
            }
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

        Color borderColor = IsRebinding
            ? new Color(255, 200, 80)
            : new Color(255, 255, 255) * 0.2f;

        spriteBatch.Draw(Primitives.Pixel, new Rectangle(
            Slot.X + borderSize,
            Slot.Y + borderSize,
            Slot.Width - 2 * borderSize,
            Slot.Height - 2 * borderSize), borderColor
        );
        spriteBatch.Draw(Primitives.Pixel, Slot, bg);

        nameLabel.Draw(spriteBatch);
        keyLabel.Draw(spriteBatch);
    }
}
