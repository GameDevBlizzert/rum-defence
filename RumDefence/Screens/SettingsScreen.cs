using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RumDefence;

public class SettingsScreen : Screen
{
    private Screen previous;

    private Texture2D panelTexture;
    private Texture2D buttonTexture;
    private Texture2D pixel;
    private SpriteFont font;

    private SimpleButton backButton;

    private Rectangle panelRect;
    private MouseState prevMouse;

    private const int PanelLeft = 560;
    private const int PanelTop = 190;
    private const int PanelWidth = 800;
    private const int PanelHeight = 620;

    public SettingsScreen(ScreenManager manager, Screen previous) : base(manager)
    {
        this.previous = previous;
    }

    public override void Load()
    {
        var content = RumGame.Instance.Content;
        font = content.Load<SpriteFont>("Fonts/KenneyFuture");
        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel");
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button");

        pixel = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        panelRect = new Rectangle(PanelLeft, PanelTop, PanelWidth, PanelHeight);

        int backX = PanelLeft + (PanelWidth - 200) / 2;
        backButton = new SimpleButton(buttonTexture, font, "Back",
            new Vector2(backX, PanelTop + PanelHeight - 110),
            new Vector2(200, 70));
        backButton.OnClick = () => manager.SetScreen(previous);
    }

    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            manager.SetScreen(previous);
            return;
        }

        var mouse = Mouse.GetState();
        var mousePos = ScreenManager.GetMousePositionScaled();

        UpdateSlider(GetMusicTrack(), mouse, mousePos, v => AudioManager.Instance.MusicVolume = v);
        UpdateSlider(GetSoundTrack(), mouse, mousePos, v => AudioManager.Instance.SoundVolume = v);

        backButton.Update(gameTime);
        prevMouse = mouse;
    }

    private void UpdateSlider(Rectangle track, MouseState mouse, Vector2 mousePos, System.Action<float> setter)
    {
        var hitArea = new Rectangle(track.X - 15, track.Y - 15, track.Width + 30, track.Height + 30);
        if (mouse.LeftButton == ButtonState.Pressed &&
            hitArea.Contains(new Point((int)mousePos.X, (int)mousePos.Y)))
        {
            float t = MathHelper.Clamp((mousePos.X - track.X) / (float)track.Width, 0f, 1f);
            setter(t);
        }
    }

    private Rectangle GetMusicTrack() =>
        new Rectangle(PanelLeft + 100, PanelTop + 260, PanelWidth - 200, 12);

    private Rectangle GetSoundTrack() =>
        new Rectangle(PanelLeft + 100, PanelTop + 420, PanelWidth - 200, 12);

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (previous != null)
            previous.Draw(spriteBatch);
        else
            RumGame.Instance.GraphicsDevice.Clear(Color.DarkSlateGray);

        spriteBatch.Draw(pixel,
            new Rectangle(0, 0, RumGame.VirtualWidth, RumGame.VirtualHeight),
            Color.Black * 0.5f);

        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        var title = "Settings";
        var titleSize = font.MeasureString(title);
        spriteBatch.DrawString(font, title,
            new Vector2(PanelLeft + (PanelWidth - titleSize.X) / 2f, PanelTop + 40),
            Color.Black);

        DrawSlider(spriteBatch, "Music Volume", AudioManager.Instance.MusicVolume, GetMusicTrack());
        DrawSlider(spriteBatch, "Sound Volume", AudioManager.Instance.SoundVolume, GetSoundTrack());

        backButton.Draw(spriteBatch);
    }

    private void DrawSlider(SpriteBatch spriteBatch, string label, float value, Rectangle track)
    {
        spriteBatch.DrawString(font, label, new Vector2(track.X, track.Y - 44), Color.Black);

        var pct = $"{(int)(value * 100)}%";
        var pctSize = font.MeasureString(pct);
        spriteBatch.DrawString(font, pct, new Vector2(track.Right - pctSize.X, track.Y - 44), Color.Black);

        spriteBatch.Draw(pixel, track, new Color(170, 170, 170));

        int filledWidth = (int)(track.Width * value);
        if (filledWidth > 0)
            spriteBatch.Draw(pixel, new Rectangle(track.X, track.Y, filledWidth, track.Height), new Color(70, 130, 200));

        int thumbSize = 28;
        int thumbX = track.X + filledWidth - thumbSize / 2;
        int thumbY = track.Y + track.Height / 2 - thumbSize / 2;
        spriteBatch.Draw(pixel, new Rectangle(thumbX, thumbY, thumbSize, thumbSize), Color.White);
        spriteBatch.Draw(pixel, new Rectangle(thumbX + 3, thumbY + 3, thumbSize - 6, thumbSize - 6), new Color(40, 100, 180));
    }
}
