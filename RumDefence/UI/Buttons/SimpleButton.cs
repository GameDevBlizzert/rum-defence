using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class SimpleButton : Button
{
    private FantasyPanel _panel;
    private SpriteFont _font;
    private string _text;
    private bool _isDisabled;
    private int _borderStyle;

    public bool IsDisabled
    {
        get => _isDisabled;
        set => _isDisabled = value;
    }

    public SimpleButton(SpriteFont font, string text, Vector2 position, Vector2 size, int borderStyle = 0)
    {
        _font = font;
        _text = text;
        _borderStyle = borderStyle;
        _panel = new FantasyPanel(borderStyle);

        SetBounds(new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)size.X,
            (int)size.Y
        ));
    }

    public SimpleButton(SpriteFont font, string text, Vector2 position, int borderStyle = 0)
    {
        _font = font;
        _text = text;
        _borderStyle = borderStyle;
        _panel = new FantasyPanel(borderStyle);

        var textSize = font.MeasureString(text);
        var padding = 32;

        SetBounds(new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)textSize.X + padding,
            (int)textSize.Y + padding
        ));
    }

    public override void SetBounds(Rectangle rect)
    {
        bounds = rect;
        Position = new Vector2(rect.X, rect.Y);
        _panel.SetBounds(rect);
    }

    protected override bool IsClickable() => !_isDisabled;

    public override void Draw(SpriteBatch spriteBatch)
    {
        var textSize = _font.MeasureString(_text);
        var centerX = bounds.X + (bounds.Width - textSize.X) / 2;
        var centerY = bounds.Y + (bounds.Height - textSize.Y) / 2;
        var textPos = new Vector2(centerX, centerY);

        if (_isDisabled)
        {
            // Disabled: plain text in dark gray
            spriteBatch.DrawString(_font, _text, textPos, Color.DarkGray);
        }
        else if (isSelected)
        {
            // Active/Selected: white background with border and black text
            _panel.DrawCenterFill = true;
            _panel.Tint = Color.White;
            _panel.Draw(spriteBatch);
            spriteBatch.DrawString(_font, _text, textPos, Color.Black);
        }
        else if (isHovering)
        {
            // Hover: show border only with white text
            _panel.DrawCenterFill = false;
            _panel.Tint = Color.White;
            _panel.Draw(spriteBatch);
            spriteBatch.DrawString(_font, _text, textPos, Color.White);
        }
        else
        {
            // Normal: plain white text, no background or border
            spriteBatch.DrawString(_font, _text, textPos, Color.White);
        }
    }

    public void SetText(string text)
    {
        _text = text;
    }

    public string GetText() => _text;
}
