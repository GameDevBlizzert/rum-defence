using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class BuildMenu
{
    private Rectangle panelRect;

    private (IconButton button, TowerData data)[] towerButtons;
    private IconButton wallButton;
    private IconButton removeButton;
    private IconButton speedButton;
    private IconButton pauseMenuButton;

    private readonly Texture2D playIcon;
    private readonly Texture2D pauseIcon;
    private readonly Texture2D fastForwardIcon;
    private readonly Texture2D highlightArrowIcon;
    private float highlightPulseTimer;
    public System.Action OnSpeedRequested;
    public System.Action OnMenuRequested;

    private GamePlaybackState playbackState = GamePlaybackState.Normal;

    private BuildManager buildManager;
    private LevelProgressSystem progress;

    private HudHealthBar healthBar;
    private Texture2D coinIcon;
    public TowerData HoveredTowerData { get; private set; }
    public bool IsWallHovered { get; private set; }
    public TowerType? FreeTowerType { get; set; }
    public TowerType? HighlightedTower { get; set; }

    private int coinAreaY;
    private int coinIconSize;
    private int panelX;

    private readonly Func<TowerData, bool> isTowerAvailable;
    private Texture2D wallIcon;
    private Texture2D removeIcon;
    private Rectangle buttonSourceRect;
    private int towerStartY;
    private int buttonX;
    private int panelY;
    private int panelHeight;
    private const int Spacing = 8;

    private const int PanelWidth = 180;
    private const int ButtonWidth = PanelWidth - 20;
    private const int ButtonHeight = 60;

    public BuildMenu(BuildManager buildManager, LevelProgressSystem progress, Func<TowerData, bool> isTowerAvailable = null)
    {
        this.buildManager = buildManager;
        this.progress = progress;
        this.isTowerAvailable = isTowerAvailable;

        var content = RumGame.Instance.Content;
        wallIcon = content.Load<Texture2D>("Art/Themes/Grass/Walls/wall");
        removeIcon = content.Load<Texture2D>("KenneyUIPack/PNG/Blue/Default/icon_cross");

        panelX = 20;
        panelY = 20;
        panelHeight = RumGame.VirtualHeight - 40;
        panelRect = new Rectangle(panelX, panelY, PanelWidth, panelHeight);

        coinIcon = content.Load<Texture2D>("Art/UI/Coin");

        const int barMargin = 8;
        int fontHeight = (int)Primitives.Font.MeasureString("0").Y;
        int barHeight = fontHeight + 8;
        coinIconSize = fontHeight;
        int coinHeight = fontHeight;

        int currentY = panelY + 70;
        buttonX = panelX + (PanelWidth - ButtonWidth) / 2;

        var healthBarBounds = new Rectangle(panelX + barMargin, currentY, PanelWidth - barMargin * 2, barHeight);
        healthBar = new HudHealthBar(progress, healthBarBounds);
        currentY += barHeight + Spacing;

        coinAreaY = currentY;
        currentY += coinHeight + Spacing;

        towerStartY = currentY;
        buttonSourceRect = new Rectangle(0, 0, 64, 64);

        BuildTowerAndUtilityButtons();

        playIcon = CreatePlayIcon(RumGame.Instance.GraphicsDevice);
        pauseIcon = CreatePauseIcon(RumGame.Instance.GraphicsDevice);
        fastForwardIcon = CreateFastForwardIcon(RumGame.Instance.GraphicsDevice);
        highlightArrowIcon = CreateHighlightArrowIcon(RumGame.Instance.GraphicsDevice);

        int speedButtonY = panelY + panelHeight - (ButtonHeight + Spacing) * 2 - 20;
        speedButton = new IconButton(Primitives.ButtonTexture, fastForwardIcon, new Vector2(buttonX, speedButtonY), new Vector2(ButtonWidth, ButtonHeight));
        speedButton.BackgroundSourceRect = buttonSourceRect;
        speedButton.OnClick = () => OnSpeedRequested?.Invoke();

        int pauseMenuButtonY = panelY + panelHeight - ButtonHeight - 20;
        pauseMenuButton = new IconButton(Primitives.ButtonTexture, pauseIcon, new Vector2(buttonX, pauseMenuButtonY), new Vector2(ButtonWidth, ButtonHeight));
        pauseMenuButton.BackgroundSourceRect = buttonSourceRect;
        pauseMenuButton.OnClick = () => OnMenuRequested?.Invoke();
    }

    public void SetPlaybackState(GamePlaybackState state)
    {
        playbackState = state;
    }

    private void BuildTowerAndUtilityButtons()
    {
        var content = RumGame.Instance.Content;
        var availableTowers = TowerFactory.All.Where(d => isTowerAvailable == null || isTowerAvailable(d)).ToArray();

        int currentY = towerStartY;
        towerButtons = new (IconButton, TowerData)[availableTowers.Length];
        for (int i = 0; i < availableTowers.Length; i++)
        {
            var data = availableTowers[i];
            var icon = content.Load<Texture2D>(data.IconTexturePath);
            var btn = new IconButton(Primitives.ButtonTexture, icon, new Vector2(buttonX, currentY), new Vector2(ButtonWidth, ButtonHeight));
            btn.BackgroundSourceRect = buttonSourceRect;
            btn.OnClick = () => buildManager.SetTowerMode(data);
            btn.CostLabel = data.Cost.ToString();
            towerButtons[i] = (btn, data);
            currentY += ButtonHeight + Spacing;
        }

        wallButton = new IconButton(Primitives.ButtonTexture, wallIcon, new Vector2(buttonX, currentY), new Vector2(ButtonWidth, ButtonHeight));
        wallButton.BackgroundSourceRect = buttonSourceRect;
        wallButton.OnClick = () => buildManager.SetMode(BuildMode.Wall);
        wallButton.CostLabel = BuildManager.WallCost.ToString();
        currentY += ButtonHeight + Spacing;

        removeButton = new IconButton(Primitives.ButtonTexture, removeIcon, new Vector2(buttonX, currentY), new Vector2(ButtonWidth, ButtonHeight));
        removeButton.BackgroundSourceRect = buttonSourceRect;
        removeButton.BaseTint = new Color(220, 70, 70);
        removeButton.OnClick = () => buildManager.SetMode(BuildMode.Remove);
    }

    public void RefreshAvailableTowers()
    {
        BuildTowerAndUtilityButtons();
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

    private static Texture2D CreateHighlightArrowIcon(GraphicsDevice graphicsDevice)
    {
        const int w = 28, h = 20;
        var data = new Color[w * h];
        int midY = h / 2;
        const int headWidth = 12;
        const int headMaxHalfHeight = 8;
        const int shaftHalfHeight = 2;

        for (int y = midY - shaftHalfHeight; y <= midY + shaftHalfHeight; y++)
            for (int x = headWidth; x < w; x++)
                data[y * w + x] = Color.White;

        // Triangle tip at x = 0 (left, pointing at the button), widening into the shaft
        for (int x = 0; x < headWidth; x++)
        {
            int halfHeight = x * headMaxHalfHeight / (headWidth - 1);
            for (int y = midY - halfHeight; y <= midY + halfHeight; y++)
                if (y >= 0 && y < h)
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

    private const string WallDescription =
        "Blocks the path and buys you time. Troops will batter\n" +
        "away at a wall and can break straight through it if it's\n" +
        "left to take damage - and if there's a way around, they'll\n" +
        "simply walk around it instead. Keep walls repaired and\n" +
        "backed up by towers.";

    private void DrawHoverInfo(SpriteBatch spriteBatch)
    {
        if (HoveredTowerData == null && !IsWallHovered)
            return;

        Vector2 mousePos = ScreenManager.GetMousePositionScaled();

        const float titleScale = 0.75f;
        const float statScale = 0.6f;
        const float descScale = 0.55f;
        const int padding = 20;
        const int spacing = 34;

        string title = HoveredTowerData != null ? HoveredTowerData.Label + " LVL 1" : "Wall";
        string description = HoveredTowerData != null ? HoveredTowerData.Description : WallDescription;
        int statCount = HoveredTowerData != null ? 4 : 2;

        const int topPadding = 24;
        const int bottomPadding = 20;

        int width = 340;
        int statsBlockHeight = (statCount + 1) * spacing;
        var descSize = !string.IsNullOrEmpty(description) ? Primitives.Font.MeasureString(description) * descScale : Vector2.Zero;
        int descGap = !string.IsNullOrEmpty(description) ? 16 : 0;

        int requiredWidth = (int)descSize.X + padding * 2;
        if (requiredWidth > width)
            width = requiredWidth;

        int height = topPadding + statsBlockHeight + descGap + (int)descSize.Y + bottomPadding;

        int x = (int)mousePos.X + 24;
        int y = (int)mousePos.Y + 24;

        if (x + width > RumGame.VirtualWidth)
            x = (int)mousePos.X - width - 24;

        if (y + height > RumGame.VirtualHeight)
            y = (int)mousePos.Y - height - 24;

        var rect = new Rectangle(x, y, width, height);

        NineSlice.Draw(spriteBatch, Primitives.PanelTexture, rect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        int startY = rect.Y + topPadding;

        spriteBatch.DrawString(Primitives.Font, title, new Vector2(rect.X + padding, startY), Primitives.FontColor, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        if (HoveredTowerData != null)
        {
            spriteBatch.DrawString(Primitives.Font, $"DAM: {HoveredTowerData.Damage}", new Vector2(rect.X + padding, startY + spacing), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Primitives.Font, $"RNG: {(int)HoveredTowerData.Range}", new Vector2(rect.X + padding, startY + spacing * 2), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Primitives.Font, $"SPD: {HoveredTowerData.FireRate:F1}/s", new Vector2(rect.X + padding, startY + spacing * 3), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Primitives.Font, $"Cost: {HoveredTowerData.Cost} coins", new Vector2(rect.X + padding, startY + spacing * 4), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        }
        else
        {
            spriteBatch.DrawString(Primitives.Font, $"HP: {Wall.BaseMaxHealth}", new Vector2(rect.X + padding, startY + spacing), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Primitives.Font, $"Cost: {BuildManager.WallCost} coins", new Vector2(rect.X + padding, startY + spacing * 2), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        }

        if (!string.IsNullOrEmpty(description))
        {
            int descY = startY + statsBlockHeight + descGap;
            spriteBatch.DrawString(Primitives.Font, description, new Vector2(rect.X + padding, descY), Primitives.FontColor, 0f, Vector2.Zero, descScale, SpriteEffects.None, 0f);
        }
    }

    private static readonly string[] TowerSlotActions =
    [
        "BuildTower1", "BuildTower2", "BuildTower3", "BuildTower4", "BuildTower5"
    ];

    private void HandleTowerSlotShortcuts()
    {
        for (int i = 0; i < towerButtons.Length && i < TowerSlotActions.Length; i++)
        {
            if (InputManager.Instance.IsActionJustPressed(TowerSlotActions[i]))
                buildManager.SetTowerMode(towerButtons[i].data);
        }
    }

    public void Update(GameTime gameTime)
    {
        HandleTowerSlotShortcuts();

        var mode = buildManager.GetMode();
        var selectedData = buildManager.SelectedTowerData;

        foreach (var (button, data) in towerButtons)
            button.SetSelected(mode == BuildMode.Tower && selectedData == data);

        wallButton.SetSelected(mode == BuildMode.Wall);
        removeButton.SetSelected(mode == BuildMode.Remove);

        bool isPaused = playbackState == GamePlaybackState.Paused;

        foreach (var (button, data) in towerButtons)
        {
            int effectiveCost = data.Type == FreeTowerType ? 0 : data.Cost;
            button.CostLabel = effectiveCost.ToString();
            button.IsDisabled = isPaused || progress.CoinsRemaining < effectiveCost;
        }

        highlightPulseTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

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

    private void DrawHighlightArrow(SpriteBatch spriteBatch, Rectangle buttonBounds)
    {
        const int arrowWidth = 28;
        const int arrowHeight = 20;
        float bounce = (float)Math.Sin(highlightPulseTimer * 5f) * 8f;

        var rect = new Rectangle(
            buttonBounds.Right + 12 + (int)bounce,
            buttonBounds.Center.Y - arrowHeight / 2,
            arrowWidth,
            arrowHeight);

        spriteBatch.Draw(highlightArrowIcon, rect, Color.Gold);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        NineSlice.Draw(spriteBatch, Primitives.PanelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

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

        foreach (var (button, data) in towerButtons)
        {
            button.Draw(spriteBatch);

            if (data.Type == HighlightedTower)
                DrawHighlightArrow(spriteBatch, button.Bounds);
        }

        wallButton.Draw(spriteBatch);
        removeButton.Draw(spriteBatch);
        speedButton.Draw(spriteBatch);
        pauseMenuButton.Draw(spriteBatch);

        DrawHoverInfo(spriteBatch);
    }
}
