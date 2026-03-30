using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class HudButton
{
    public Rectangle Bounds;

    private Texture2D texture;
    private string text;
    private SpriteFont font;

    private System.Action onClick;

    public HudButton(Rectangle bounds, Texture2D texture, string text, SpriteFont font, System.Action onClick)
    {
        Bounds = bounds;
        this.texture = texture;
        this.text = text;
        this.font = font;
        this.onClick = onClick;
    }

    public void Update(Vector2 mousePos, bool isClick)
    {
        if (Bounds.Contains(mousePos) && isClick)
        {
            onClick?.Invoke();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, Bounds, Color.White);

        if (font != null && !string.IsNullOrEmpty(text))
        {
            var size = font.MeasureString(text);
            var pos = new Vector2(
                Bounds.Center.X - size.X / 2,
                Bounds.Center.Y - size.Y / 2
            );

            spriteBatch.DrawString(font, text, pos, Color.Black);
        }
    }
}