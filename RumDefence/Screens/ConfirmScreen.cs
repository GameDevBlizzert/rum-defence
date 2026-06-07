using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box;
using RumDefence.UI.Box.Components;
using System;

namespace RumDefence;

public class ConfirmScreen : Screen
{
    private string message;
    private Action onConfirm;
    private Screen previous;
    private const int messageWidth = 600;

    private ButtonBox yesButton;
    private ButtonBox noButton;

    private Box panel;
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
        yesButton = new ButtonBox(Primitives.ButtonTexture, "Yes");
        noButton = new ButtonBox(Primitives.ButtonTexture, "No");
        yesButton.Arrange(new Rectangle(yesX, buttonY, (int)buttonSize.X, (int)buttonSize.Y));
        noButton.Arrange(new Rectangle(noX, buttonY, (int)buttonSize.X, (int)buttonSize.Y));

        panel = new Box();
        panel.AddBackground(new ImageBox(Primitives.PanelTexture));
        panel.Arrange(panelRect);

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

        panel.Draw(spriteBatch);

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
