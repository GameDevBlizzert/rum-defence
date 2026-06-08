using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box;
using RumDefence.UI.Box;
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
        var buttonSize = new Vector2(200, 70);

        var messageText = new TextItem(message);

        var buttonRow = new Box
        {
            Direction = Direction.Column,
            AlignX = Align.Center,
            AlignY = Align.Center,
            Gap = 30,
            Padding = 0
        };

        yesButton = new ButtonBox(Primitives.ButtonTexture, "Yes", size: buttonSize);
        noButton = new ButtonBox(Primitives.ButtonTexture, "No", size: buttonSize);
        buttonRow.Add(yesButton);
        buttonRow.Add(noButton);

        panel = new Box
        {
            Direction = Direction.Row,
            AlignX = Align.Center,
            AlignY = Align.Center,
            Gap = 30,
            Padding = 30
        };
        panel.AddBackground(new ImageBox(Primitives.PanelTexture));
        panel.Add(messageText);
        panel.Add(buttonRow);

        var contentHeight = (int)panel.Measure().Y + 2 * panel.Padding;
        var panelX = (RumGame.VirtualWidth - panelWidth) / 2;
        var panelY = (RumGame.VirtualHeight - contentHeight) / 2;
        panelRect = new Rectangle(panelX, panelY, panelWidth, contentHeight);
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
        panel.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        previous.Draw(spriteBatch);

        spriteBatch.Draw(
            Primitives.Pixel,
            new Rectangle(0, 0, RumGame.VirtualWidth, RumGame.VirtualHeight),
            Color.Black * 0.3f);

        panel.Draw(spriteBatch);
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
