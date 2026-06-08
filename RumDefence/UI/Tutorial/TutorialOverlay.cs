using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class TutorialOverlay
{
    private static readonly string[] IntroSteps =
    [
        "Welcome to Rum Defence!\nPirates are coming for your rum barrel,\nbuild towers and walls to stop them!",
        "Use the build panel on the left to place\ntowers and walls. Towers attack\nenemies automatically.\nWalls block and redirect them.",
        "Click a highlighted tile to place.\nHold CTRL while placing to keep the build\nmode active.",
        "Earn coins from defeated enemies and\nspend them on more defences.\nGood luck, captain!",
    ];

    private int currentStep = 0;
    private bool introComplete = false;

    private bool waveNotified = false;
    private bool waveNotificationActive = false;
    private float waveNotificationTimer = 0f;
    private const float WaveNotificationDuration = 5f;

    private Texture2D panelTexture;
    private SimpleButton nextButton;

    private const float TextScale = 0.62f;

    private const int PanelWidth = 900;
    private const int PanelHeight = 250;
    private static readonly int PanelX = (RumGame.VirtualWidth - PanelWidth) / 2;
    private static readonly int PanelY = RumGame.VirtualHeight - PanelHeight - 30;

    private const int WaveNotifWidth = 760;
    private const int WaveNotifHeight = 200;
    private static readonly int WaveNotifX = (RumGame.VirtualWidth - WaveNotifWidth) / 2;
    private const int WaveNotifY = 80;

    public bool IsIntroActive => !introComplete;

    public TutorialOverlay()
    {
        var content = RumGame.Instance.Content;
        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel");
        var buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button");

        nextButton = new SimpleButton(
            buttonTexture,
            "Next",
            new Vector2(PanelX + PanelWidth - 168, PanelY + PanelHeight - 68),
            new Vector2(140, 52)
        );
        nextButton.TextScale = TextScale;
        nextButton.OnClick = Advance;
    }

    public void NotifyWaveStarted()
    {
        if (waveNotified) return;
        waveNotified = true;
        waveNotificationActive = true;
        waveNotificationTimer = WaveNotificationDuration;
    }

    public void Update(GameTime gameTime)
    {
        if (waveNotificationActive)
        {
            waveNotificationTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (waveNotificationTimer <= 0f)
                waveNotificationActive = false;
        }

        if (!introComplete)
        {
            nextButton.Text = currentStep == IntroSteps.Length - 1 ? "Got it!" : "Next";
            nextButton.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!introComplete)
            DrawIntroPanel(spriteBatch);

        if (waveNotificationActive)
            DrawWaveNotification(spriteBatch);
    }

    private void DrawIntroPanel(SpriteBatch spriteBatch)
    {
        var panelRect = new Rectangle(PanelX, PanelY, PanelWidth, PanelHeight);
        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        spriteBatch.DrawString(Primitives.Font, $"{currentStep + 1} / {IntroSteps.Length}",
            new Vector2(PanelX + 22, PanelY + 18),
            Color.White * 0.5f, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);

        spriteBatch.DrawString(Primitives.Font, IntroSteps[currentStep],
            new Vector2(PanelX + 28, PanelY + 58),
            Primitives.FontColor, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);

        nextButton.Draw(spriteBatch);
    }

    private void DrawWaveNotification(SpriteBatch spriteBatch)
    {
        float alpha = MathHelper.Clamp(waveNotificationTimer, 0f, 1f);
        NineSlice.Draw(spriteBatch, panelTexture,
            new Rectangle(WaveNotifX, WaveNotifY, WaveNotifWidth, WaveNotifHeight),
            new Rectangle(0, 0, 128, 128), 20, Color.White * alpha);

        spriteBatch.DrawString(Primitives.Font,
            "The first wave is incoming!\nTowers fire automatically at nearby\nenemies.",
            new Vector2(WaveNotifX + 30, WaveNotifY + 30),
            Primitives.FontColor * alpha, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
    }

    private void Advance()
    {
        currentStep++;
        if (currentStep >= IntroSteps.Length)
            introComplete = true;
    }
}