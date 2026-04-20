using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class FantasyButton : Button
{
    private FantasyPanel _panel;
    private SpriteFont _font;
    private string _text;
    private Color _textColor = Color.White;
    private Color _textColorDisabled = Color.DarkGray;
    private Color _baseTint = Color.White;
    private bool _isDisabled;

    public bool IsDisabled
    {
        get => _isDisabled;
        set => _isDisabled = value;
    }
    public Color BaseTint
    {
        get => _baseTint;
        set => _baseTint = value;
    }

    public Color TextColor
    {
        get => _textColor;
        set => _textColor = value;
    }

    public Color TextColorDisabled
    {
        get => _textColorDisabled;
        set => _textColorDisabled = value;
    }

    public FantasyButton(string text, SpriteFont font, Vector2 position, Vector2 size, int borderStyle = 0)
    {
        _text = text;
        _font = font;
        _panel = new FantasyPanel(borderStyle);

        SetBounds(new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)size.X,
            (int)size.Y
        ));
    }

    public FantasyButton(string text, SpriteFont font, Vector2 position, int borderStyle = 0)
    {
        _text = text;
        _font = font;
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
        // Determine tint based on state
        Color tint;
        if (_isDisabled)
            tint = new Color(80, 80, 80);
        else if (isSelected)
            tint = Color.Multiply(_baseTint, 0.7f);
        else if (isHovering)
            tint = Color.Multiply(_baseTint, 0.85f);
        else
            tint = _baseTint;

        _panel.Tint = tint;
        _panel.Draw(spriteBatch);

        // Draw text
        var textSize = _font.MeasureString(_text);
        var contentArea = _panel.GetContentArea();

        var textPos = new Vector2(
            contentArea.X + (contentArea.Width - textSize.X) / 2,
            contentArea.Y + (contentArea.Height - textSize.Y) / 2
        );

        var color = _isDisabled ? _textColorDisabled : _textColor;
        spriteBatch.DrawString(_font, _text, textPos, color);
    }

    public void SetText(string text)
    {
        _text = text;
    }

    public string GetText() => _text;
}
