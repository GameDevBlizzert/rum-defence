using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class SimpleButton : Button
{
    private Texture2D texture;
    private SpriteFont font;
    private string text;
    private bool isSelected;

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

    public override void Draw(SpriteBatch spriteBatch)
    {
        Color color = Color.White;

        if (isSelected)
            color = Color.Gray;
        else if (isHovering)
            color = Color.LightGray;

        spriteBatch.Draw(texture, bounds, color);

        var textSize = font.MeasureString(text);

        var textPos = new Vector2(
            bounds.X + (bounds.Width - textSize.X) / 2,
            bounds.Y + (bounds.Height - textSize.Y) / 2
        );

        spriteBatch.DrawString(font, text, textPos, Color.Black);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
    }
}