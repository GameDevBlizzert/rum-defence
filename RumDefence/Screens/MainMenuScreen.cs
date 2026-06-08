using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box;
using RumDefence.UI.Box;

namespace RumDefence;

public class MainMenuScreen : Screen
{
    private ButtonBox levelsButton;
    private ButtonBox settingsButton;
    private ButtonBox quitButton;

    private Box panel;
    private Rectangle panelRect;
    private float elapsedSeconds;
    public MainMenuScreen(ScreenManager manager) : base(manager) { }
    public override void Load()
    {
        panelRect = new Rectangle(500, 200, 900, 700);

        var buttonSize = new Vector2(300, 100);

        levelsButton = new ButtonBox(Primitives.ButtonTexture, "Levels", size: buttonSize);
        settingsButton = new ButtonBox(Primitives.ButtonTexture, "Settings", size: buttonSize);
        quitButton = new ButtonBox(Primitives.ButtonTexture, "Quit", size: buttonSize);

        panel = new Box
        {
            Direction = Direction.Row,
            AlignX = Align.Center,
            AlignY = Align.Center,
            Gap = 40,
            Padding = 60
        };
        panel.AddBackground(new ImageBox(Primitives.PanelTexture));
        panel.Add(levelsButton);
        panel.Add(settingsButton);
        panel.Add(quitButton);
        panel.Arrange(panelRect);

        levelsButton.OnClick = () =>
        {
            manager.SetScreen(new ThemeSelectScreen(manager));
        };

        settingsButton.OnClick = () =>
        {
            manager.SetScreen(new SettingsScreen(manager, this));
        };

        quitButton.OnClick = () =>
        {
            manager.SetScreen(new ConfirmScreen(
                manager,
                this,
                "Are you sure you want to quit?",
                () => RumGame.Instance.Exit()
            ));
        };

        // Play main menu theme music
        AudioManager.Instance.PlayBackgroundMusic();
    }

    public override void Update(GameTime gameTime)
    {
        elapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
        panel.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);

        LoadingSplashScreen.DrawBackground(spriteBatch, RumGame.VirtualWidth, RumGame.VirtualHeight, elapsedSeconds);
        LoadingSplashScreen.DrawWater(spriteBatch, RumGame.VirtualWidth, RumGame.VirtualHeight, elapsedSeconds);

        panel.Draw(spriteBatch);
    }
}
