using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class MainMenuScreen : Screen
{
    private SimpleButton levelsButton;
    private SimpleButton settingsButton;
    private SimpleButton quitButton;

    private Rectangle panelRect;
    private float elapsedSeconds;
    public MainMenuScreen(ScreenManager manager) : base(manager) { }
    public override void Load()
    {
        panelRect = new Rectangle(500, 200, 900, 700);

        levelsButton = new SimpleButton(Primitives.ButtonTexture, "Levels", new Vector2(800, 300), new Vector2(300, 100));
        settingsButton = new SimpleButton(Primitives.ButtonTexture, "Settings", new Vector2(800, 450), new Vector2(300, 100));
        quitButton = new SimpleButton(Primitives.ButtonTexture, "Quit", new Vector2(800, 600), new Vector2(300, 100));

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
        levelsButton.Update(gameTime);
        settingsButton.Update(gameTime);
        quitButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);

        LoadingSplashScreen.DrawBackground(spriteBatch, RumGame.VirtualWidth, RumGame.VirtualHeight, elapsedSeconds);
        LoadingSplashScreen.DrawWater(spriteBatch, RumGame.VirtualWidth, RumGame.VirtualHeight, elapsedSeconds);

        NineSlice.Draw(spriteBatch, Primitives.PanelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        levelsButton.Draw(spriteBatch);
        settingsButton.Draw(spriteBatch);
        quitButton.Draw(spriteBatch);
    }
}
