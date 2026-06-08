using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box;

namespace RumDefence;

public class BuildMenu
{
    private Rectangle panelRect;
    private Box panel;
    private Box hoverInfoPanel;

    private readonly (IconButtonBox button, TowerData data)[] towerButtons;
    private IconButtonBox wallButton;
    private IconButtonBox removeButton;
    private IconButtonBox speedButton;
    private IconButtonBox pauseMenuButton;

    private readonly Texture2D playIcon;
    private readonly Texture2D pauseIcon;
    private readonly Texture2D fastForwardIcon;
    public System.Action OnSpeedRequested;
    public System.Action OnMenuRequested;

    private GamePlaybackState playbackState = GamePlaybackState.Normal;

    private BuildManager buildManager;
    private LevelProgressSystem progress;

    private HudHealthBar healthBar;
    private Texture2D coinIcon;
    public TowerData HoveredTowerData { get; private set; }
    public bool IsWallHovered { get; private set; }

    private int coinAreaY;
    private int coinIconSize;
    private int panelX;

    private const int PanelWidth = 180;
    private const int ButtonWidth = PanelWidth - 20;
    private const int ButtonHeight = 60;

    public BuildMenu(BuildManager buildManager, LevelProgressSystem progress)
    {
        this.buildManager = buildManager;
        this.progress = progress;

        var content = RumGame.Instance.Content;
        var wallIcon = content.Load<Texture2D>("Art/Themes/Grass/Walls/wall");
        var removeIcon = content.Load<Texture2D>("KenneyUIPack/PNG/Blue/Default/icon_cross");

        panelX = 20;
        int panelY = 20;
        int panelHeight = RumGame.VirtualHeight - 40;
        panelRect = new Rectangle(panelX, panelY, PanelWidth, panelHeight);

        coinIcon = content.Load<Texture2D>("Art/UI/Coin");

        const int barMargin = 8;
        int fontHeight = (int)Primitives.Font.MeasureString("0").Y;
        int barHeight = fontHeight + 8;
        coinIconSize = fontHeight;
        int coinHeight = fontHeight;
        const int spacing = 8;

        int currentY = panelY + 70;
        int buttonX = panelX + (PanelWidth - ButtonWidth) / 2;

        var healthBarBounds = new Rectangle(panelX + barMargin, currentY, PanelWidth - barMargin * 2, barHeight);
        healthBar = new HudHealthBar(progress, healthBarBounds);
        currentY += barHeight + spacing;

        coinAreaY = currentY;
        currentY += coinHeight + spacing;

        var buttonSourceRect = new Rectangle(0, 0, 64, 64);

        var allTowers = TowerFactory.All;
        towerButtons = new (IconButtonBox, TowerData)[allTowers.Length];
        for (int i = 0; i < allTowers.Length; i++)
        {
            var data = allTowers[i];
            var icon = content.Load<Texture2D>(data.IconTexturePath);
            var btn = new IconButtonBox(Primitives.ButtonTexture, icon, buttonSourceRect);
            btn.Arrange(new Rectangle(buttonX, currentY, ButtonWidth, ButtonHeight));
            btn.OnClick = () => buildManager.SetTowerMode(data);
            btn.CostLabel = data.Cost.ToString();
            towerButtons[i] = (btn, data);
            currentY += ButtonHeight + spacing;
        }

        wallButton = new IconButtonBox(Primitives.ButtonTexture, wallIcon, buttonSourceRect);
        wallButton.Arrange(new Rectangle(buttonX, currentY, ButtonWidth, ButtonHeight));
        wallButton.OnClick = () => buildManager.SetMode(BuildMode.Wall);
        wallButton.CostLabel = BuildManager.WallCost.ToString();
        currentY += ButtonHeight + spacing;

        removeButton = new IconButtonBox(Primitives.ButtonTexture, removeIcon, buttonSourceRect);
        removeButton.Arrange(new Rectangle(buttonX, currentY, ButtonWidth, ButtonHeight));
        removeButton.BaseTint = new Color(220, 70, 70);
        removeButton.OnClick = () => buildManager.SetMode(BuildMode.Remove);

        playIcon = CreatePlayIcon(RumGame.Instance.GraphicsDevice);
        pauseIcon = CreatePauseIcon(RumGame.Instance.GraphicsDevice);
        fastForwardIcon = CreateFastForwardIcon(RumGame.Instance.GraphicsDevice);

        int speedButtonY = panelY + panelHeight - (ButtonHeight + spacing) * 2 - 20;
        speedButton = new IconButtonBox(Primitives.ButtonTexture, fastForwardIcon, buttonSourceRect);
        speedButton.Arrange(new Rectangle(buttonX, speedButtonY, ButtonWidth, ButtonHeight));
        speedButton.OnClick = () => OnSpeedRequested?.Invoke();

        int pauseMenuButtonY = panelY + panelHeight - ButtonHeight - 20;
        pauseMenuButton = new IconButtonBox(Primitives.ButtonTexture, pauseIcon, buttonSourceRect);
        pauseMenuButton.Arrange(new Rectangle(buttonX, pauseMenuButtonY, ButtonWidth, ButtonHeight));
        pauseMenuButton.OnClick = () => OnMenuRequested?.Invoke();

        panel = new Box();
        panel.AddBackground(new ImageBox(Primitives.PanelTexture));
        panel.Arrange(panelRect);
    }

    public void SetPlaybackState(GamePlaybackState state)
    {
        playbackState = state;
    }

    private static Texture2D CreatePlayIcon(GraphicsDevice graphicsDevice)
    {
        const int w = 24, h = 14;
        var data = new Color[w * h];

        int midY = h / 2;
        for (int y = 0; y < h; y++)
        {
            int halfWidth = (int)Math.Round((midY - Math.Abs(y - midY)) * 0.85f);
            int startX = 4;
            for (int x = startX; x < startX + halfWidth * 2 && x < w - 2; x++)
                data[y * w + x] = Color.White;
        }

        var tex = new Texture2D(graphicsDevice, w, h);
        tex.SetData(data);
        return tex;
    }

    private static Texture2D CreatePauseIcon(GraphicsDevice graphicsDevice)
    {
        const int w = 24, h = 14;
        var data = new Color[w * h];

        for (int y = 2; y < h - 2; y++)
        {
            for (int x = 4; x < 8; x++)
                data[y * w + x] = Color.White;

            for (int x = 16; x < 20; x++)
                data[y * w + x] = Color.White;
        }

        var tex = new Texture2D(graphicsDevice, w, h);
        tex.SetData(data);
        return tex;
    }

    private static Texture2D CreateFastForwardIcon(GraphicsDevice graphicsDevice)
    {
        const int w = 24, h = 14;
        var data = new Color[w * h];

        int midY = h / 2;
        for (int y = 0; y < h; y++)
        {
            int halfWidth = (int)Math.Round((midY - Math.Abs(y - midY)) * 0.85f);

            int leftStartX = 1;
            for (int x = leftStartX; x < leftStartX + halfWidth * 2 && x < 11; x++)
                data[y * w + x] = Color.White;

            int rightStartX = 12;
            for (int x = rightStartX; x < rightStartX + halfWidth * 2 && x < w - 1; x++)
                data[y * w + x] = Color.White;
        }

        var tex = new Texture2D(graphicsDevice, w, h);
        tex.SetData(data);
        return tex;
    }

    public Vector2 GetCoinTargetPosition()
    {
        var size = Primitives.Font.MeasureString(progress.CoinsRemaining.ToString());
        const int iconTextGap = 4;
        float totalWidth = coinIconSize + iconTextGap + size.X;
        float coinRowX = panelX + (PanelWidth - totalWidth) / 2f;
        return new Vector2(coinRowX + coinIconSize / 2f, coinAreaY + coinIconSize / 2f);
    }

    private void DrawHoverInfo(SpriteBatch spriteBatch)
    {
        if (HoveredTowerData == null && !IsWallHovered)
            return;

        Vector2 mousePos = ScreenManager.GetMousePositionScaled();

        int width = 340;
        int height = 220;

        int x = (int)mousePos.X + 24;
        int y = (int)mousePos.Y + 24;

        if (x + width > RumGame.VirtualWidth)
            x = (int)mousePos.X - width - 24;

        if (y + height > RumGame.VirtualHeight)
            y = (int)mousePos.Y - height - 24;

        var rect = new Rectangle(x, y, width, height);

        if (hoverInfoPanel == null)
        {
            hoverInfoPanel = new Box();
            hoverInfoPanel.AddBackground(new ImageBox(Primitives.PanelTexture));
        }
        hoverInfoPanel.Arrange(rect);
        hoverInfoPanel.Draw(spriteBatch);

        float titleScale = 0.75f;
        float statScale = 0.6f;
        int startY = rect.Y + 24;
        int spacing = 34;

        if (HoveredTowerData != null)
        {
            spriteBatch.DrawString(
                Primitives.Font,
                HoveredTowerData.Label + " LVL 1",
                new Vector2(rect.X + 20, startY),
                Primitives.FontColor,
                0f,
                Vector2.Zero,
                titleScale,
                SpriteEffects.None,
                0f
            );

            spriteBatch.DrawString(Primitives.Font, $"DAM: {HoveredTowerData.Damage}", new Vector2(rect.X + 20, startY + spacing), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Primitives.Font, $"RNG: {(int)HoveredTowerData.Range}", new Vector2(rect.X + 20, startY + spacing * 2), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Primitives.Font, $"SPD: {HoveredTowerData.FireRate:F1}/s", new Vector2(rect.X + 20, startY + spacing * 3), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Primitives.Font, $"Cost: {HoveredTowerData.Cost} coins", new Vector2(rect.X + 20, startY + spacing * 4), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        }
        else if (IsWallHovered)
        {
            spriteBatch.DrawString(
                Primitives.Font,
                "Wall",
                new Vector2(rect.X + 20, startY),
                Primitives.FontColor,
                0f,
                Vector2.Zero,
                titleScale,
                SpriteEffects.None,
                0f
            );

            spriteBatch.DrawString(Primitives.Font, $"HP: {Wall.BaseMaxHealth}", new Vector2(rect.X + 20, startY + spacing), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Primitives.Font, $"Cost: {BuildManager.WallCost} coins", new Vector2(rect.X + 20, startY + spacing * 2), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        }
    }

    public void Update(GameTime gameTime)
    {
        var mode = buildManager.GetMode();
        var selectedData = buildManager.SelectedTowerData;

        foreach (var (button, data) in towerButtons)
            button.IsSelected = mode == BuildMode.Tower && selectedData == data;

        wallButton.IsSelected = mode == BuildMode.Wall;
        removeButton.IsSelected = mode == BuildMode.Remove;

        bool isPaused = playbackState == GamePlaybackState.Paused;

        foreach (var (button, data) in towerButtons)
            button.IsDisabled = isPaused || progress.CoinsRemaining < data.Cost;

        wallButton.IsDisabled = isPaused || progress.CoinsRemaining < BuildManager.WallCost;
        removeButton.IsDisabled = isPaused;

        speedButton.IconTexture = playbackState switch
        {
            GamePlaybackState.Normal => fastForwardIcon,
            GamePlaybackState.FastForward => playIcon,
            _ => playIcon
        };

        HoveredTowerData = null;

        foreach (var (button, data) in towerButtons)
        {
            button.Update(gameTime);

            if (button.IsHovered)
                HoveredTowerData = data;
        }

        foreach (var (button, _) in towerButtons)
            button.Update(gameTime);

        wallButton.Update(gameTime);
        removeButton.Update(gameTime);
        speedButton.Update(gameTime);
        pauseMenuButton.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        panel.Draw(spriteBatch);

        healthBar.Draw(spriteBatch);

        var coinText = progress.CoinsRemaining.ToString();
        var coinSize = Primitives.Font.MeasureString(coinText);
        const int iconTextGap = 4;
        float totalWidth = coinIconSize + iconTextGap + coinSize.X;
        float coinRowX = panelX + (PanelWidth - totalWidth) / 2f;
        var iconRect = new Rectangle((int)coinRowX, coinAreaY, coinIconSize, coinIconSize);

        spriteBatch.Draw(coinIcon, iconRect, Color.White);

        float textY = coinAreaY + (coinIconSize - coinSize.Y) / 2f;

        spriteBatch.DrawString(
            Primitives.Font,
            coinText,
            new Vector2(coinRowX + coinIconSize + iconTextGap, textY),
            Primitives.FontColor
        );

        foreach (var (button, _) in towerButtons)
            button.Draw(spriteBatch);

        wallButton.Draw(spriteBatch);
        removeButton.Draw(spriteBatch);
        speedButton.Draw(spriteBatch);
        pauseMenuButton.Draw(spriteBatch);

        DrawHoverInfo(spriteBatch);
    }
}
