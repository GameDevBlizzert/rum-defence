using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class InfoPopupOverlay
{
    private readonly record struct PopupEntry(string Title, string Body);

    private readonly Queue<PopupEntry> pending = new();
    private PopupEntry? current;

    private Texture2D panelTexture;
    private SimpleButton continueButton;

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
        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel");
        var buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button");

        continueButton = new SimpleButton(
            buttonTexture,
            "Continue",
            new Vector2(PanelX + PanelWidth - 188, PanelY + PanelHeight - 68),
            new Vector2(160, 52)
        );
        continueButton.TextScale = BodyScale;
        continueButton.OnClick = Advance;
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

        var entry = current.Value;
        var panelRect = new Rectangle(PanelX, PanelY, PanelWidth, PanelHeight);
        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        spriteBatch.DrawString(Primitives.Font, entry.Title,
            new Vector2(PanelX + 28, PanelY + 24),
            Primitives.FontColor, 0f, Vector2.Zero, TitleScale, SpriteEffects.None, 0f);

        spriteBatch.DrawString(Primitives.Font, entry.Body,
            new Vector2(PanelX + 28, PanelY + 78),
            Primitives.FontColor, 0f, Vector2.Zero, BodyScale, SpriteEffects.None, 0f);

        continueButton.Draw(spriteBatch);
    }

    private void Advance()
    {
        current = pending.Count > 0 ? pending.Dequeue() : null;
    }
}
