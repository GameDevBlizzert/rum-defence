using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace RumDefence;

public class Button : UIElement
{
    private Texture2D texture;
    private SpriteFont font;
    private string text;

    private Rectangle bounds;

    private bool isHovering;

    public Action OnClick;

    private bool wasPressed;

    public Button(Texture2D texture, SpriteFont font, string text, Vector2 position)
    {
        this.texture = texture;
        this.font = font;
        this.text = text;
        this.Position = position;

        bounds = new Rectangle(
            (int)position.X,
            (int)position.Y,
            texture.Width,
            texture.Height
        );
    }

    public Button(Texture2D texture, SpriteFont font, string text, Vector2 position, Vector2 size)
    {
        this.texture = texture;
        this.font = font;
        this.text = text;
        this.Position = position;

        bounds = new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)size.X,
            (int)size.Y
        );
    }

    public override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        var mouseRect = new Rectangle(mouse.X, mouse.Y, 1, 1);

        isHovering = mouseRect.Intersects(bounds);

        bool isPressed = mouse.LeftButton == ButtonState.Pressed;

        if (isHovering && !isPressed && wasPressed)
        {
            OnClick?.Invoke();
        }

        wasPressed = isPressed;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Color color = isHovering ? Color.LightGray : Color.White;

        spriteBatch.Draw(texture, bounds, color);

        var textSize = font.MeasureString(text);

        var textPos = new Vector2(
            bounds.X + (bounds.Width - textSize.X) / 2,
            bounds.Y + (bounds.Height - textSize.Y) / 2
        );

        spriteBatch.DrawString(font, text, textPos, Color.Black);
    }
}