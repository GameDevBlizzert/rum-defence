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

        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel");
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button");

        panelRect = new Rectangle(600, 300, 700, 400);

        yesButton = new SimpleButton(buttonTexture, "Yes", new Vector2(700, 550), new Vector2(200, 100));
        noButton = new SimpleButton(buttonTexture, "No", new Vector2(1000, 550), new Vector2(200, 100));

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
        spriteBatch.Draw(Primitives.Pixel,
            new Rectangle(0, 0, RumGame.VirtualWidth, RumGame.VirtualHeight),
            Color.Black * 0.5f);

        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        spriteBatch.DrawString(Primitives.Font, message, new Vector2(650, 400), Primitives.FontColor);

        yesButton.Draw(spriteBatch);
        noButton.Draw(spriteBatch);
    }

}