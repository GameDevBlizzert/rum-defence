using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence;

public class ConfirmScreen : Screen
{
    private string message;
    private Action onConfirm;
    private Screen previous;

    private Texture2D panelTexture;
    private Texture2D buttonTexture;
    private SpriteFont font;

    private Button yesButton;
    private Button noButton;

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
        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel_blue");
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button_blue");

        panelRect = new Rectangle(600, 300, 700, 400);

        yesButton = new Button(buttonTexture, font, "Yes", new Vector2(700, 550), new Vector2(200, 100));
        noButton = new Button(buttonTexture, font, "No", new Vector2(1000, 550), new Vector2(200, 100));

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

        spriteBatch.Draw(panelTexture, panelRect, Color.White);

        spriteBatch.DrawString(font, message, new Vector2(650, 400), Color.Black);

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