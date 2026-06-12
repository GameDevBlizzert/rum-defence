using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box;

namespace RumDefence;

public class BuildMenu
{
    private Rectangle panelRect;
    private Box panel;
    private Box hoverInfoPanel;

    private (IconButtonBox button, TowerData data)[] towerButtons;
    private IconButtonBox wallButton;
    private IconButtonBox removeButton;
    private IconButtonBox speedButton;
    private IconButtonBox pauseMenuButton;

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

    private ProgressBarBox healthBar;
    private ImageBox coinIconBox;
    private TextItem coinText;
    private Box contentBox;
    private Box coinRow;
    public TowerData HoveredTowerData { get; private set; }
    public bool IsWallHovered { get; private set; }
    public TowerType? FreeTowerType { get; set; }
    public TowerType? HighlightedTower { get; set; }

    private readonly Func<TowerData, bool> isTowerAvailable;
    private const int Spacing = 8;

    private const int PanelWidth = 180;
    private const int ButtonWidth = PanelWidth - 20;
    private const int ButtonHeight = 60;
    private Vector2 buttonSize;
    private Rectangle buttonSourceRect;

    public BuildMenu(BuildManager buildManager, LevelProgressSystem progress, Func<TowerData, bool> isTowerAvailable = null)
    {
        this.buildManager = buildManager;
        this.progress = progress;
        this.isTowerAvailable = isTowerAvailable;

        var content = RumGame.Instance.Content;
        var wallIcon = content.Load<Texture2D>("Art/Themes/Grass/Walls/wall");
        var removeIcon = content.Load<Texture2D>("KenneyUIPack/PNG/Blue/Default/icon_cross");

        int panelX = 0;
        int panelY = 0;
        var panelHeight = RumGame.VirtualHeight;
        panelRect = new Rectangle(panelX, panelY, PanelWidth, panelHeight);

        var coinIcon = content.Load<Texture2D>("Art/UI/Coin");

        int fontHeight = (int)Primitives.Font.MeasureString("0").Y;
        int barHeight = fontHeight + 8;
        int coinIconSize = fontHeight;
        const int iconTextGap = 4;
        const int spacing = 8;

        buttonSize = new Vector2(ButtonWidth, ButtonHeight);
        buttonSourceRect = new Rectangle(0, 0, 64, 64);

        healthBar = new ProgressBarBox
        {
            Size = new Vector2(ButtonWidth, barHeight),
            Padding = 24,
            TrackColor = new Color(60, 0, 0),
            FillColorFunc = pct => Color.Lerp(Color.Red, Color.LimeGreen, pct)
        };
        healthBar.AddBackground(new ImageBox(Primitives.ButtonTexture));

        coinIconBox = new ImageBox(coinIcon, coinIconSize, coinIconSize);
        coinText = new TextItem("");
        coinRow = new Box { Direction = Direction.Column, Gap = iconTextGap, Padding = iconTextGap };
        coinRow.Add(coinIconBox);
        coinRow.Add(coinText);

        var availableTowers = TowerFactory.All.Where(d => isTowerAvailable == null || isTowerAvailable(d)).ToArray();
        towerButtons = new (IconButtonBox, TowerData)[availableTowers.Length];
        for (int i = 0; i < availableTowers.Length; i++)
        {
            var data = availableTowers[i];
            var icon = content.Load<Texture2D>(data.IconTexturePath);
            var btn = new IconButtonBox(Primitives.ButtonTexture, icon, buttonSourceRect);
            btn.Size = buttonSize;
            btn.OnClick = () => buildManager.SetTowerMode(data);
            btn.CostLabel = data.Cost.ToString();
            towerButtons[i] = (btn, data);
        }

        wallButton = new IconButtonBox(Primitives.ButtonTexture, wallIcon, buttonSourceRect);
        wallButton.Size = buttonSize;
        wallButton.OnClick = () => buildManager.SetMode(BuildMode.Wall);
        wallButton.CostLabel = BuildManager.WallCost.ToString();

        removeButton = new IconButtonBox(Primitives.ButtonTexture, removeIcon, buttonSourceRect);
        removeButton.Size = buttonSize;
        removeButton.BaseTint = new Color(220, 70, 70);
        removeButton.OnClick = () => buildManager.SetMode(BuildMode.Remove);

        playIcon = CreatePlayIcon(RumGame.Instance.GraphicsDevice);
        pauseIcon = CreatePauseIcon(RumGame.Instance.GraphicsDevice);
        fastForwardIcon = CreateFastForwardIcon(RumGame.Instance.GraphicsDevice);
        highlightArrowIcon = CreateHighlightArrowIcon(RumGame.Instance.GraphicsDevice);

        speedButton = new IconButtonBox(Primitives.ButtonTexture, fastForwardIcon, buttonSourceRect);
        speedButton.Size = buttonSize;
        speedButton.OnClick = () => OnSpeedRequested?.Invoke();

        pauseMenuButton = new IconButtonBox(Primitives.ButtonTexture, pauseIcon, buttonSourceRect);
        pauseMenuButton.Size = buttonSize;
        pauseMenuButton.OnClick = () => OnMenuRequested?.Invoke();

        contentBox = new Box { Direction = Direction.Row, Gap = spacing, Padding = 0, AlignX = Align.Center };
        contentBox.Add(healthBar);
        contentBox.Add(coinRow);
        foreach (var (button, _) in towerButtons)
            contentBox.Add(button);
        contentBox.Add(wallButton);
        contentBox.Add(removeButton);

        var bottomContent = new Box { Direction = Direction.Row, Gap = spacing, Padding = 0, AlignX = Align.Center };
        bottomContent.Add(speedButton);
        bottomContent.Add(pauseMenuButton);

        panel = new Box { Direction = Direction.Row, AlignY = Align.Between, Padding = 8, AlignX = Align.Center };
        panel.AddBackground(new ImageBox(Primitives.PanelTexture));
        panel.Add(contentBox);
        panel.Add(bottomContent);
        panel.Arrange(panelRect);
    }

    public void SetPlaybackState(GamePlaybackState state)
    {
        playbackState = state;
    }

    public void Refresh()
    {
        var content = RumGame.Instance.Content;
        var availableTowers = TowerFactory.All.Where(d => isTowerAvailable == null || isTowerAvailable(d)).ToArray();
        towerButtons = new (IconButtonBox, TowerData)[availableTowers.Length];
        for (int i = 0; i < availableTowers.Length; i++)
        {
            var data = availableTowers[i];
            var icon = content.Load<Texture2D>(data.IconTexturePath);
            var btn = new IconButtonBox(Primitives.ButtonTexture, icon, buttonSourceRect);
            btn.Size = buttonSize;
            btn.OnClick = () => buildManager.SetTowerMode(data);
            btn.CostLabel = data.Cost.ToString();
            towerButtons[i] = (btn, data);
        }

        contentBox.Clear();
        contentBox.Add(healthBar);
        contentBox.Add(coinRow);
        foreach (var (button, _) in towerButtons)
            contentBox.Add(button);
        contentBox.Add(wallButton);
        contentBox.Add(removeButton);

        panel.Arrange(panelRect);
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
        return new Vector2(coinIconBox.Slot.Center.X, coinIconBox.Slot.Center.Y);
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

        const float statScale = 0.6f;
        const float descScale = 0.55f;
        const int padding = 20;
        const int spacing = 34;

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

        if (hoverInfoPanel == null)
        {
            hoverInfoPanel = new Box();
            hoverInfoPanel.AddBackground(new ImageBox(Primitives.PanelTexture));
        }
        hoverInfoPanel.Arrange(rect);
        hoverInfoPanel.Draw(spriteBatch);

        int startY = rect.Y + 24;

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
            button.IsSelected = mode == BuildMode.Tower && selectedData == data;

        wallButton.IsSelected = mode == BuildMode.Wall;
        removeButton.IsSelected = mode == BuildMode.Remove;

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

        healthBar.Progress = progress.LivesTotal > 0
            ? MathHelper.Clamp((float)progress.LivesRemaining / progress.LivesTotal, 0f, 1f)
            : 0f;
        healthBar.Label = $"{progress.LivesRemaining}";
        coinText.Text = progress.CoinsRemaining.ToString();

        panel.Update(gameTime);

        HoveredTowerData = null;
        IsWallHovered = false;

        foreach (var (button, data) in towerButtons)
            if (button.IsHovered)
                HoveredTowerData = data;

        if (wallButton.IsHovered)
            IsWallHovered = true;
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
        panel.Draw(spriteBatch);

        foreach (var (button, data) in towerButtons)
            if (data.Type == HighlightedTower)
                DrawHighlightArrow(spriteBatch, button.Slot);

        DrawHoverInfo(spriteBatch);
    }
}
