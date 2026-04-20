using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence;

public class ConfirmScreen : Screen
{
    private string message;
    private Action onConfirm;
    private Screen previous;

    private SpriteFont font;

    private SimpleButton yesButton;
    private SimpleButton noButton;

    private Rectangle panelRect;

    public ConfirmScreen(ScreenManager manager, Screen previous, string message, Action onConfirm) : base(manager)
    {
        this.previous = previous;
        this.message = message;
        this.onConfirm = onConfirm;
    }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");

        panelRect = new Rectangle(600, 300, 700, 400);

        yesButton = new SimpleButton(font, "Yes", new Vector2(700, 550), new Vector2(200, 100));
        noButton = new SimpleButton(font, "No", new Vector2(1000, 550), new Vector2(200, 100));

        yesButton.OnClick = () =>
        {
            onConfirm.Invoke();
        };

        noButton.OnClick = () =>
        {
            manager.SetScreen(previous);
        };
    }

    public override void Update(GameTime gameTime)
    {
        yesButton.Update(gameTime);
        noButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(GetPixel(spriteBatch),
            new Rectangle(0, 0, RumGame.VirtualWidth, RumGame.VirtualHeight),
            Color.Black * 0.5f);

        var textSize = font.MeasureString(message);
        var textX = (RumGame.VirtualWidth - textSize.X) / 2;
        spriteBatch.DrawString(font, message, new Vector2(textX, 400), Color.White);

        yesButton.Draw(spriteBatch);
        noButton.Draw(spriteBatch);
    }

    private Texture2D pixel;
    private Texture2D GetPixel(SpriteBatch spriteBatch)
    {
        if (pixel == null)
        {
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }
        return pixel;
    }
}
