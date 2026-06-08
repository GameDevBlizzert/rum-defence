using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box;

namespace RumDefence;

public class SettingsScreen : Screen
{
    private const float VolumeStep = 0.05f;

    private Screen previous;

    private ButtonBox keyBindingsButton;
    private ButtonBox backButton;

    private TextItem titleText;
    private TextItem musicLabel;
    private TextItem soundLabel;

    private ProgressBarBox musicSlider;
    private ProgressBarBox soundSlider;

    private Box panel;
    private Rectangle panelRect;

    private const int PanelLeft = 560;
    private const int PanelTop = 190;
    private const int PanelWidth = 800;
    private const int PanelHeight = 720;

    public SettingsScreen(ScreenManager manager, Screen previous) : base(manager)
    {
        this.previous = previous;
    }

    public override void Load()
    {
        panelRect = new Rectangle(PanelLeft, PanelTop, PanelWidth, PanelHeight);

        var sliderSize = new Vector2(360, 24);
        var stepButtonSize = new Vector2(60, 60);
        var navButtonSize = new Vector2(400, 70);

        titleText = new TextItem("Settings");

        musicLabel = new TextItem("");
        soundLabel = new TextItem("");

        musicSlider = new ProgressBarBox
        {
            TrackColor = new Color(170, 170, 170),
            FillColor = new Color(70, 130, 200),
            Size = sliderSize
        };
        soundSlider = new ProgressBarBox
        {
            TrackColor = new Color(170, 170, 170),
            FillColor = new Color(70, 130, 200),
            Size = sliderSize
        };

        var musicMinusButton = new ButtonBox(Primitives.ButtonTexture, "-", size: stepButtonSize);
        var musicPlusButton = new ButtonBox(Primitives.ButtonTexture, "+", size: stepButtonSize);
        var soundMinusButton = new ButtonBox(Primitives.ButtonTexture, "-", size: stepButtonSize);
        var soundPlusButton = new ButtonBox(Primitives.ButtonTexture, "+", size: stepButtonSize);

        var musicRow = new Box { Direction = Direction.Column, Gap = 16, Padding = 0 };
        musicRow.Add(musicMinusButton);
        musicRow.Add(musicSlider);
        musicRow.Add(musicPlusButton);

        var soundRow = new Box { Direction = Direction.Column, Gap = 16, Padding = 0 };
        soundRow.Add(soundMinusButton);
        soundRow.Add(soundSlider);
        soundRow.Add(soundPlusButton);

        keyBindingsButton = new ButtonBox(Primitives.ButtonTexture, "Key Bindings", size: navButtonSize);
        backButton = new ButtonBox(Primitives.ButtonTexture, "Back", size: new Vector2(200, 70));

        panel = new Box { Direction = Direction.Row, Gap = 30, Padding = 40 };
        panel.AddBackground(new ImageBox(Primitives.PanelTexture));
        panel.Add(titleText);
        panel.Add(musicLabel);
        panel.Add(musicRow);
        panel.Add(soundLabel);
        panel.Add(soundRow);
        panel.Add(keyBindingsButton);
        panel.Add(backButton);
        panel.Arrange(panelRect);

        musicMinusButton.OnClick = () => AudioManager.Instance.MusicVolume -= VolumeStep;
        musicPlusButton.OnClick = () => AudioManager.Instance.MusicVolume += VolumeStep;
        soundMinusButton.OnClick = () => AudioManager.Instance.SoundVolume -= VolumeStep;
        soundPlusButton.OnClick = () => AudioManager.Instance.SoundVolume += VolumeStep;

        keyBindingsButton.OnClick = () =>
        {
            SaveManager.CurrentSave.MusicVolume = AudioManager.Instance.MusicVolume;
            SaveManager.CurrentSave.SfxVolume = AudioManager.Instance.SoundVolume;
            SaveManager.Save();
            manager.SetScreen(new KeyBindingsScreen(manager, this));
        };

        backButton.OnClick = () =>
        {
            SaveManager.CurrentSave.MusicVolume = AudioManager.Instance.MusicVolume;
            SaveManager.CurrentSave.SfxVolume = AudioManager.Instance.SoundVolume;
            SaveManager.Save();
            manager.SetScreen(previous);
        };
    }

    public override void Update(GameTime gameTime)
    {
        if (InputManager.Instance.IsActionJustPressed("Pause"))
        {
            SaveManager.CurrentSave.MusicVolume = AudioManager.Instance.MusicVolume;
            SaveManager.CurrentSave.SfxVolume = AudioManager.Instance.SoundVolume;
            SaveManager.Save();
            manager.SetScreen(previous);
            return;
        }

        musicLabel.Text = $"Music Volume  {(int)(AudioManager.Instance.MusicVolume * 100)}%";
        soundLabel.Text = $"Sound Volume  {(int)(AudioManager.Instance.SoundVolume * 100)}%";
        musicSlider.Progress = AudioManager.Instance.MusicVolume;
        soundSlider.Progress = AudioManager.Instance.SoundVolume;

        panel.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (previous != null)
            previous.Draw(spriteBatch);
        else
            RumGame.Instance.GraphicsDevice.Clear(Color.DarkSlateGray);

        spriteBatch.Draw(Primitives.Pixel,
            new Rectangle(0, 0, RumGame.VirtualWidth, RumGame.VirtualHeight),
            Color.Black * 0.5f);

        panel.Draw(spriteBatch);
    }
}
