using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class SimpleButton : Button
{
    private Texture2D texture;
    private string text;
    public string Text { get => text; set => text = value; }

    public bool IsDisabled { get; set; }

    public Color BaseTint { get; set; } = Color.White;
    public float TextScale { get; set; } = 1f;

    public SimpleButton(Texture2D texture, string text, Vector2 position, Vector2 size)
    {
        this.texture = texture;
        this.text = text;

        SetBounds(new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)size.X,
            (int)size.Y
        ));
    }

    public SimpleButton(Texture2D texture, string text, Vector2 position)
    {
        this.texture = texture;
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

        var textSize = Primitives.Font.MeasureString(text) * TextScale;

        var textPos = new Vector2(
            bounds.X + (bounds.Width - textSize.X) / 2,
            bounds.Y + (bounds.Height - textSize.Y) / 2
        );

        var textColor = IsDisabled ? Color.DarkGray : Primitives.FontColor;
        spriteBatch.DrawString(Primitives.Font, text, textPos, textColor, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
    }

}