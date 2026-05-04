using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class SimpleButton : Button
{
    private Texture2D texture;
    private SpriteFont font;
    private string text;

    public bool IsDisabled { get; set; }

    public Color BaseTint { get; set; } = Color.White;

    public SimpleButton(Texture2D texture, SpriteFont font, string text, Vector2 position, Vector2 size)
    {
        this.texture = texture;
        this.font = font;
        this.text = text;

        SetBounds(new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)size.X,
            (int)size.Y
        ));
    }

    public SimpleButton(Texture2D texture, SpriteFont font, string text, Vector2 position)
    {
        this.texture = texture;
        this.font = font;
        this.text = text;

        SetBounds(new Rectangle(
            (int)position.X,
            (int)position.Y,
            texture.Width,
            texture.Height
        ));
    }

    protected override bool IsClickable() => !IsDisabled;

    public override void Draw(SpriteBatch spriteBatch)
    {
        Color color;

        if (IsDisabled)
            color = new Color(80, 80, 80);
        else if (isSelected)
            color = Color.Gray;
        else if (isHovering)
            color = Color.Multiply(BaseTint, 0.8f);
        else
            color = Color.White;

        NineSlice.Draw(spriteBatch, texture, bounds, null, 10, color);

        var textSize = font.MeasureString(text);

        var textPos = new Vector2(
            bounds.X + (bounds.Width - textSize.X) / 2,
            bounds.Y + (bounds.Height - textSize.Y) / 2
        );

        var textColor = IsDisabled ? Color.DarkGray : Color.Black;
        spriteBatch.DrawString(font, text, textPos, textColor);
    }

}