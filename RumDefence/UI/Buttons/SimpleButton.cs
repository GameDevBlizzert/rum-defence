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
        Color textColor;

        if (IsDisabled)
        {
            color = new Color(80, 80, 80);
            textColor = Color.DarkGray;
        }
        else if (isPressed)
        {
            color = Color.Lerp(BaseTint, Color.Black, 0.9f);
            textColor = Color.White;
        }
        else if (isSelected)
        {
            color = Color.Lerp(BaseTint, new Color(70, 70, 70), 0.7f);
            textColor = Primitives.FontColor;
        }
        else if (isHovering)
        {
            color = Color.Lerp(BaseTint, new Color(40, 40, 40), 0.25f);
            textColor = Primitives.FontColor;
        }
        else
        {
            color = BaseTint;
            textColor = Primitives.FontColor;
        }

        NineSlice.Draw(spriteBatch, texture, bounds, null, 10, color);

        var textSize = Primitives.Font.MeasureString(text) * TextScale;

        var textPos = new Vector2(
            bounds.X + (bounds.Width - textSize.X) / 2,
            bounds.Y + (bounds.Height - textSize.Y) / 2
        );

        spriteBatch.DrawString(Primitives.Font, text, textPos, textColor, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
    }

}
