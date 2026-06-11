using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box;

namespace RumDefence;

public class InfoPopupOverlay
{
    private readonly record struct PopupEntry(string Title, string Body);

    private readonly Queue<PopupEntry> pending = new();
    private PopupEntry? current;

    private ButtonBox continueButton;
    private Box panel;
    private const float TitleScale = 0.75f;
    private const float BodyScale = 0.62f;

    private const int PanelWidth = 900;
    private const int PanelHeight = 280;
    private static readonly int PanelX = (RumGame.VirtualWidth - PanelWidth) / 2;
    private static readonly int PanelY = (RumGame.VirtualHeight - PanelHeight) / 2;
    public bool IsActive => current != null;

    public InfoPopupOverlay()
    {
        var content = RumGame.Instance.Content;

        panel = new Box();
        panel.AddBackground(new ImageBox(Primitives.PanelTexture));

        continueButton = new ButtonBox(
            Primitives.ButtonTexture,
            "Continue"
        );
        continueButton.OnClick = Advance;

        panel.Add(new TextItem(current?.Title, scale: TitleScale));
        panel.Add(new TextItem(current?.Body, scale: BodyScale));
        panel.Add(continueButton);
        panel.Arrange(new Rectangle(PanelX, PanelY, PanelWidth, PanelHeight));
    }

    public void Show(string title, string body)
    {
        pending.Enqueue(new PopupEntry(title, body));
        if (current == null)
            current = pending.Dequeue();
    }

    public void Update(GameTime gameTime)
    {
        if (current == null)
            return;

        continueButton.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (current == null)
            return;

        panel.Draw(spriteBatch);

        continueButton.Draw(spriteBatch);
    }

    private void Advance()
    {
        current = pending.Count > 0 ? pending.Dequeue() : null;
    }
}
