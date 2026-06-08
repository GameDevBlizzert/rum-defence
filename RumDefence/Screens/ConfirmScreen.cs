using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence;

public class ConfirmScreen : Screen
{
    private string message;
    private Action onConfirm;
    private Screen previous;
    private const int messageWidth = 600;

    private SimpleButton yesButton;
    private SimpleButton noButton;

    private Rectangle panelRect;

    public ConfirmScreen(ScreenManager manager, Screen previous, string message, Action onConfirm) : base(manager)
    {
        this.previous = previous;
        this.onConfirm = onConfirm;
        this.message = WrapText(Primitives.Font, message, messageWidth);
    }

    public override void Load()
    {
        const int panelWidth = 700;
        const int padding = 30;
        var buttonSize = new Vector2(200, 70);
        var messageHeight = (int)Primitives.Font.MeasureString(message).Y;
        var panelHeight = padding + messageHeight + padding + (int)buttonSize.Y + padding;
        var panelX = (RumGame.VirtualWidth - panelWidth) / 2;
        var panelY = (RumGame.VirtualHeight - panelHeight) / 2;
        panelRect = new Rectangle(panelX, panelY, panelWidth, panelHeight);

        var buttonY = panelY + padding + messageHeight + padding;
        var yesX = panelX + panelWidth / 2 - (int)buttonSize.X - padding / 2;
        var noX = panelX + panelWidth / 2 + padding / 2;
        yesButton = new SimpleButton(Primitives.ButtonTexture, "Yes", new Vector2(yesX, buttonY), buttonSize);
        noButton = new SimpleButton(Primitives.ButtonTexture, "No", new Vector2(noX, buttonY), buttonSize);

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
        previous.Draw(spriteBatch);

        spriteBatch.Draw(
            Primitives.Pixel,
            new Rectangle(0, 0, RumGame.VirtualWidth, RumGame.VirtualHeight),
            Color.Black * 0.3f);

        NineSlice.Draw(spriteBatch, Primitives.PanelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        spriteBatch.DrawString(Primitives.Font, message, new Vector2(panelRect.X + (panelRect.Width - messageWidth) / 2, panelRect.Y + 10), Primitives.FontColor);

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
