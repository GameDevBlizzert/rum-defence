using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box;

namespace RumDefence;

public class PauseScreen : Screen
{
    private Screen previousScreen;
    private bool pausedDueToFocusLoss;

    private ButtonBox resumeButton;
    private ButtonBox settingsButton;
    private ButtonBox menuButton;

    private Box panel;
    private Rectangle panelRect;

    public PauseScreen(ScreenManager manager, Screen previous, bool focusLoss = false) : base(manager)
    {
        previousScreen = previous;
        pausedDueToFocusLoss = focusLoss;
    }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        // Only switch to pause music when the player manually paused.
        // For focus-loss pauses, audio is already suspended by RumGame —
        // we leave it alone so ResumeAudio() can restore it perfectly.
        if (!pausedDueToFocusLoss)
        {
            AudioManager.Instance.PlayBackgroundMusic();
        }

        panelRect = new Rectangle(500, 200, 900, 700);

        var buttonSize = new Vector2(300, 100);

        resumeButton = new ButtonBox(Primitives.ButtonTexture, "Resume", size: buttonSize);
        settingsButton = new ButtonBox(Primitives.ButtonTexture, "Settings", size: buttonSize);
        menuButton = new ButtonBox(Primitives.ButtonTexture, "Main Menu", size: buttonSize);

        panel = new Box
        {
            Direction = Direction.Row,
            AlignX = Align.Center,
            AlignY = Align.Center,
            Gap = 50,
            Padding = 60
        };
        panel.AddBackground(new ImageBox(Primitives.PanelTexture));
        panel.Add(resumeButton);
        panel.Add(settingsButton);
        panel.Add(menuButton);
        panel.Arrange(panelRect);

        resumeButton.OnClick = () =>
        {
            // Return to the game — restore gameplay music.
            AudioManager.Instance.PlayBackgroundMusic("menu_music");
            manager.SetScreen(previousScreen);
        };

        settingsButton.OnClick = () =>
        {
            manager.SetScreen(new SettingsScreen(manager, this));
        };

        menuButton.OnClick = () =>
        {
            AudioManager.Instance.StopBackgroundMusic();
            manager.SetScreen(new ConfirmScreen(
                manager,
                this,
                "Are you sure you want to return to the main menu?",
                () => manager.SetScreen(new MainMenuScreen(manager))
            ));
        };
    }

    public override void Update(GameTime gameTime)
    {
        panel.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        previousScreen.Draw(spriteBatch);

        spriteBatch.Draw(
            Primitives.Pixel,
            new Rectangle(0, 0, RumGame.VirtualWidth, RumGame.VirtualHeight),
            Color.Black * 0.3f);

        panel.Draw(spriteBatch);
    }
}
