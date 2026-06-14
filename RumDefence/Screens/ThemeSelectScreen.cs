using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.Gameplay.Levels.Dev;
using RumDefence.Gameplay.Levels.Ghost;
using RumDefence.Levels.Ghost;
using RumDefence.Levels.Grass;
using RumDefence.UI.Box;
using RumDefence.Levels.Infinity;
using RumDefence.Levels.OneHp;

namespace RumDefence;

public class ThemeSelectScreen : Screen
{
    private ButtonBox grassButton;
    private ButtonBox stoneButton;
    private ButtonBox oneHpButton;
    private ButtonBox infinityButton;
    private ButtonBox devButton;
    private ButtonBox backButton;

    private Box panel;

    private float elapsedSeconds;
    private bool grassCompleted;

    public ThemeSelectScreen(ScreenManager manager) : base(manager) { }

    public override void Load()
    {
        grassCompleted = SaveManager.HasCompletedAllGrassLevels();

        grassButton = new ButtonBox(Primitives.ButtonTexture, "Grass", size: new Vector2(300, 100));
        stoneButton = new ButtonBox(Primitives.ButtonTexture, "Stone", size: new Vector2(300, 100));
        oneHpButton = new ButtonBox(Primitives.ButtonTexture, "1 HP", size: new Vector2(300, 100));
        infinityButton = new ButtonBox(Primitives.ButtonTexture, "Infinity", size: new Vector2(300, 100));

        backButton = new ButtonBox(Primitives.ButtonTexture, "Back");
        backButton.Arrange(new Rectangle(20, 20, 200, 80));

        var row1 = new Box { Direction = Direction.Column, Gap = 20, AlignX = Align.Center, AlignY = Align.Center };
        row1.Add(grassButton);
        row1.Add(stoneButton);

        var row2 = new Box { Direction = Direction.Column, Gap = 20, AlignX = Align.Center, AlignY = Align.Center };
        row2.Add(oneHpButton);
        row2.Add(infinityButton);

        bool devMode = Environment.GetEnvironmentVariable("RUM_DEV") == "true";
        if (devMode)
        {
            devButton = new ButtonBox(Primitives.ButtonTexture, "Dev", size: new Vector2(300, 100));
            devButton.OnClick = () => manager.SetScreen(new LevelSelectScreen(manager, DevLevels.All));
            row2.Add(devButton);
        }

        panel = new Box { Direction = Direction.Row, Gap = 80, AlignX = Align.Center, AlignY = Align.Center };
        panel.Add(row1);
        panel.Add(row2);

        var panelSize = panel.Measure();
        int panelX = (RumGame.VirtualWidth - (int)panelSize.X) / 2;
        int panelY = (RumGame.VirtualHeight - (int)panelSize.Y) / 2;
        panel.Arrange(new Rectangle(panelX, panelY, (int)panelSize.X, (int)panelSize.Y));

        grassButton.OnClick = () => manager.SetScreen(new LevelSelectScreen(manager, GrassLevels.All));

        stoneButton.OnClick = () =>
        {
            if (grassCompleted)
                manager.SetScreen(new LevelSelectScreen(manager, GhostLevels.All));
        };

        oneHpButton.OnClick = () =>
        {
            if (grassCompleted)
                manager.SetScreen(new LevelSelectScreen(manager, OneHpLevels.All));
        };

        infinityButton.OnClick = () =>
        {
            if (grassCompleted)
                manager.SetScreen(new LevelSelectScreen(manager, InfinityLevels.All));
        };

        stoneButton.IsDisabled = !grassCompleted;
        oneHpButton.IsDisabled = !grassCompleted;
        infinityButton.IsDisabled = !grassCompleted;

        backButton.OnClick = () => manager.SetScreen(new MainMenuScreen(manager));

        AudioManager.Instance.PlayBackgroundMusic();
    }

    public override void Update(GameTime gameTime)
    {
        elapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
        panel.Update(gameTime);
        backButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);

        LoadingSplashScreen.DrawBackground(spriteBatch, RumGame.VirtualWidth, RumGame.VirtualHeight, elapsedSeconds);
        LoadingSplashScreen.DrawWater(spriteBatch, RumGame.VirtualWidth, RumGame.VirtualHeight, elapsedSeconds);

        panel.Draw(spriteBatch);
        backButton.Draw(spriteBatch);

        if (!grassCompleted)
        {
            DrawLockedLabel(spriteBatch, stoneButton);
            DrawLockedLabel(spriteBatch, oneHpButton);
            DrawLockedLabel(spriteBatch, infinityButton);
        }
    }

    private static void DrawLockedLabel(SpriteBatch spriteBatch, ButtonBox button)
    {
        const string text = "Locked - finish the Grass campaign";
        const float scale = 0.45f;
        var size = Primitives.Font.MeasureString(text) * scale;
        var bounds = button.Slot;
        var position = new Vector2(bounds.Center.X - size.X / 2f, bounds.Bottom + 6);

        spriteBatch.DrawString(Primitives.Font, text, position, new Color(200, 80, 80), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }
}
