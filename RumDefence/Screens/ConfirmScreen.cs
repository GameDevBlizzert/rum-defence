using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence;

public class ConfirmScreen : Screen
{
    private string message;
    private Action onConfirm;
    private Screen previous;

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
        panelRect = new Rectangle(600, 300, 700, 400);

        yesButton = new SimpleButton(Primitives.ButtonTexture, "Yes", new Vector2(700, 550), new Vector2(200, 100));
        noButton = new SimpleButton(Primitives.ButtonTexture, "No", new Vector2(1000, 550), new Vector2(200, 100));

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

        NineSlice.Draw(spriteBatch, Primitives.PanelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        const int messageWidth = 600;
        var wrappedMessage = WrapText(Primitives.Font, message, messageWidth);
        spriteBatch.DrawString(Primitives.Font, wrappedMessage, new Vector2(650, 380), Primitives.FontColor);

        yesButton.Draw(spriteBatch);
        noButton.Draw(spriteBatch);
    }

    private static string WrapText(SpriteFont font, string text, float maxWidth)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
        {
            return string.Empty;
        }

        var lines = new System.Collections.Generic.List<string>();
        var currentLine = words[0];

        for (var index = 1; index < words.Length; index++)
        {
            var candidateLine = currentLine + " " + words[index];

            if (font.MeasureString(candidateLine).X <= maxWidth)
            {
                currentLine = candidateLine;
            }
            else
            {
                lines.Add(currentLine);
                currentLine = words[index];
            }
        }

        lines.Add(currentLine);
        return string.Join(Environment.NewLine, lines);
    }
}
