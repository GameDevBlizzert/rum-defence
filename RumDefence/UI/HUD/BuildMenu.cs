using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class BuildMenu
{
    private Texture2D panelTexture;
    private Texture2D pixelTexture;
    private SpriteFont font;

    private Rectangle panelRect;

    private IconButton wallButton;
    private IconButton cannonButton;
    private IconButton musketButton;
    private IconButton removeButton;
    private IconButton speedButton;
    private IconButton pauseMenuButton;

    private readonly Texture2D playIcon;
    private readonly Texture2D pauseIcon;
    private readonly Texture2D fastForwardIcon;
    private readonly Texture2D hamburgerIcon;

    public System.Action OnSpeedRequested;
    public System.Action OnMenuRequested;

    private GamePlaybackState playbackState = GamePlaybackState.Normal;

    private BuildManager buildManager;
    private LevelProgressSystem progress;

    private HudHealthBar healthBar;
    private Texture2D coinIcon;

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
        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel");
        font = content.Load<SpriteFont>("Fonts/KenneyFuture");

        var wallIcon = content.Load<Texture2D>("Art/Themes/Grass/Walls/wall");
        var cannonIcon = content.Load<Texture2D>("KenneyPiratePack/PNG/Default size/Ship parts/cannon");
        var removeIcon = content.Load<Texture2D>("KenneyUIPack/PNG/Blue/Default/icon_cross");

        pixelTexture = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
        pixelTexture.SetData(new[] { Color.White });

        panelX = 20;
        int panelY = 20;
        int panelHeight = RumGame.VirtualHeight - 40;
        panelRect = new Rectangle(panelX, panelY, PanelWidth, panelHeight);

        coinIcon = content.Load<Texture2D>("Art/UI/Coin");

        const int barMargin = 8;
        int fontHeight = (int)font.MeasureString("0").Y;
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

        var panelSourceRect = new Rectangle(0, 0, 128, 128);

        cannonButton = new IconButton(panelTexture, cannonIcon, new Vector2(buttonX, currentY), new Vector2(ButtonWidth, ButtonHeight));
        cannonButton.BackgroundSourceRect = panelSourceRect;
        cannonButton.OnClick = () => buildManager.SetMode(BuildMode.CannonTower);
        cannonButton.CostLabel = TowerFactory.Cannon.Cost.ToString();
        cannonButton.Font = font;
        currentY += ButtonHeight + spacing;

        musketButton = new IconButton(panelTexture, cannonIcon, new Vector2(buttonX, currentY), new Vector2(ButtonWidth, ButtonHeight));
        musketButton.BackgroundSourceRect = panelSourceRect;
        musketButton.OnClick = () => buildManager.SetMode(BuildMode.MusketTower);
        musketButton.CostLabel = TowerFactory.Musket.Cost.ToString();
        musketButton.Font = font;
        currentY += ButtonHeight + spacing;

        wallButton = new IconButton(panelTexture, wallIcon, new Vector2(buttonX, currentY), new Vector2(ButtonWidth, ButtonHeight));
        wallButton.BackgroundSourceRect = panelSourceRect;
        wallButton.OnClick = () => buildManager.SetMode(BuildMode.Wall);
        wallButton.CostLabel = BuildManager.WallCost.ToString();
        wallButton.Font = font;
        currentY += ButtonHeight + spacing;

        removeButton = new IconButton(panelTexture, removeIcon, new Vector2(buttonX, currentY), new Vector2(ButtonWidth, ButtonHeight));
        removeButton.BackgroundSourceRect = panelSourceRect;
        removeButton.BaseTint = new Color(220, 70, 70);
        removeButton.OnClick = () => buildManager.SetMode(BuildMode.Remove);

        playIcon = CreatePlayIcon(RumGame.Instance.GraphicsDevice);
        pauseIcon = CreatePauseIcon(RumGame.Instance.GraphicsDevice);
        fastForwardIcon = CreateFastForwardIcon(RumGame.Instance.GraphicsDevice);
        hamburgerIcon = CreateHamburgerIcon(RumGame.Instance.GraphicsDevice);

        int speedButtonY = panelY + panelHeight - (ButtonHeight + spacing) * 2 - 20;
        speedButton = new IconButton(panelTexture, fastForwardIcon, new Vector2(buttonX, speedButtonY), new Vector2(ButtonWidth, ButtonHeight));
        speedButton.BackgroundSourceRect = panelSourceRect;
        speedButton.OnClick = () => OnSpeedRequested?.Invoke();

        int pauseMenuButtonY = panelY + panelHeight - ButtonHeight - 20;
        pauseMenuButton = new IconButton(panelTexture, hamburgerIcon, new Vector2(buttonX, pauseMenuButtonY), new Vector2(ButtonWidth, ButtonHeight));
        pauseMenuButton.BackgroundSourceRect = panelSourceRect;
        pauseMenuButton.OnClick = () => OnMenuRequested?.Invoke();
    }

    public void SetPlaybackState(GamePlaybackState state)
    {
        playbackState = state;
    }

    private static Texture2D CreatePlayIcon(GraphicsDevice graphicsDevice)
    {
        const int w = 24, h = 14;
        var data = new Color[w * h];

        // Triangle: pointy right. From top to bottom, each row gets wider.
        int midY = h / 2;
        for (int y = 0; y < h; y++)
        {
            // Distance from center determines how wide this row is
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

            // Left triangle
            int leftStartX = 1;
            for (int x = leftStartX; x < leftStartX + halfWidth * 2 && x < 11; x++)
                data[y * w + x] = Color.White;

            // Right triangle
            int rightStartX = 12;
            for (int x = rightStartX; x < rightStartX + halfWidth * 2 && x < w - 1; x++)
                data[y * w + x] = Color.White;
        }

        var tex = new Texture2D(graphicsDevice, w, h);
        tex.SetData(data);
        return tex;
    }

    private static Texture2D CreateHamburgerIcon(GraphicsDevice graphicsDevice)
    {
        const int w = 24, h = 14;
        var data = new Color[w * h];

        int[] barStartYs = { 2, 6, 10 };
        const int barHeight = 2;

        foreach (int barY in barStartYs)
            for (int y = barY; y < barY + barHeight && y < h; y++)
                for (int x = 2; x < w - 2; x++)
                    data[y * w + x] = Color.White;

        var tex = new Texture2D(graphicsDevice, w, h);
        tex.SetData(data);
        return tex;
    }

    public Vector2 GetCoinTargetPosition()
    {
        var size = font.MeasureString(progress.CoinsRemaining.ToString());
        const int iconTextGap = 4;
        float totalWidth = coinIconSize + iconTextGap + size.X;
        float coinRowX = panelX + (PanelWidth - totalWidth) / 2f;
        return new Vector2(coinRowX + coinIconSize / 2f, coinAreaY + coinIconSize / 2f);
    }

    public void Update(GameTime gameTime)
    {
        var mode = buildManager.GetMode();
        cannonButton.SetSelected(mode == BuildMode.CannonTower);
        musketButton.SetSelected(mode == BuildMode.MusketTower);
        wallButton.SetSelected(mode == BuildMode.Wall);
        removeButton.SetSelected(mode == BuildMode.Remove);

        bool isPaused = playbackState == GamePlaybackState.Paused;

        cannonButton.IsDisabled = isPaused || progress.CoinsRemaining < TowerFactory.Cannon.Cost;
        musketButton.IsDisabled = isPaused || progress.CoinsRemaining < TowerFactory.Musket.Cost;
        wallButton.IsDisabled = isPaused || progress.CoinsRemaining < BuildManager.WallCost;
        removeButton.IsDisabled = isPaused;

        speedButton.IconTexture = playbackState switch
        {
            GamePlaybackState.Normal => fastForwardIcon,
            GamePlaybackState.FastForward => pauseIcon,
            _ => playIcon
        };

        cannonButton.Update(gameTime);
        musketButton.Update(gameTime);
        wallButton.Update(gameTime);
        removeButton.Update(gameTime);
        speedButton.Update(gameTime);
        pauseMenuButton.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        healthBar.Draw(spriteBatch);

        var coinText = progress.CoinsRemaining.ToString();
        var coinSize = font.MeasureString(coinText);
        const int iconTextGap = 4;
        float totalWidth = coinIconSize + iconTextGap + coinSize.X;
        float coinRowX = panelX + (PanelWidth - totalWidth) / 2f;
        var iconRect = new Rectangle((int)coinRowX, coinAreaY, coinIconSize, coinIconSize);
        spriteBatch.Draw(coinIcon, iconRect, Color.White);
        float textY = coinAreaY + (coinIconSize - coinSize.Y) / 2f;
        spriteBatch.DrawString(font, coinText, new Vector2(coinRowX + coinIconSize + iconTextGap, textY), Color.Yellow);

        cannonButton.Draw(spriteBatch);
        musketButton.Draw(spriteBatch);
        wallButton.Draw(spriteBatch);
        removeButton.Draw(spriteBatch);
        speedButton.Draw(spriteBatch);
        pauseMenuButton.Draw(spriteBatch);
    }
}
