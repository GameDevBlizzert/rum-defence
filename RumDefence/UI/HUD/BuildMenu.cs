using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class BuildMenu
{
    private Texture2D panelTexture;
    private Texture2D buttonTexture;
    private Rectangle panelRect;
    private IconButton wallButton;
    private IconButton cannonButton;
    private IconButton musketButton;
    private IconButton removeButton;
    private IconButton menuButton;

    public System.Action OnMenuRequested;

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
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button");

        var wallIcon = content.Load<Texture2D>("Art/Themes/Grass/Walls/wall");
        var cannonIcon = content.Load<Texture2D>("Art/Towers/cannon-icon");
        var musketIcon = content.Load<Texture2D>("Art/Towers/musket-icon");
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

        cannonButton = new IconButton(buttonTexture, cannonIcon, new Vector2(buttonX, currentY), new Vector2(ButtonWidth, ButtonHeight));
        cannonButton.BackgroundSourceRect = buttonSourceRect;
        cannonButton.OnClick = () => buildManager.SetMode(BuildMode.CannonTower);
        cannonButton.CostLabel = TowerFactory.Cannon.Cost.ToString();
        currentY += ButtonHeight + spacing;

        musketButton = new IconButton(buttonTexture, musketIcon, new Vector2(buttonX, currentY), new Vector2(ButtonWidth, ButtonHeight));
        musketButton.BackgroundSourceRect = buttonSourceRect;
        musketButton.OnClick = () => buildManager.SetMode(BuildMode.MusketTower);
        musketButton.CostLabel = TowerFactory.Musket.Cost.ToString();
        currentY += ButtonHeight + spacing;

        wallButton = new IconButton(buttonTexture, wallIcon, new Vector2(buttonX, currentY), new Vector2(ButtonWidth, ButtonHeight));
        wallButton.BackgroundSourceRect = buttonSourceRect;
        wallButton.OnClick = () => buildManager.SetMode(BuildMode.Wall);
        wallButton.CostLabel = BuildManager.WallCost.ToString();
        currentY += ButtonHeight + spacing;

        removeButton = new IconButton(buttonTexture, removeIcon, new Vector2(buttonX, currentY), new Vector2(ButtonWidth, ButtonHeight));
        removeButton.BackgroundSourceRect = buttonSourceRect;
        removeButton.BaseTint = new Color(220, 70, 70);
        removeButton.OnClick = () => buildManager.SetMode(BuildMode.Remove);

        int menuButtonY = panelY + panelHeight - ButtonHeight - 20;
        var hamburgerIcon = CreateHamburgerIcon(RumGame.Instance.GraphicsDevice);
        menuButton = new IconButton(buttonTexture, hamburgerIcon, new Vector2(buttonX, menuButtonY), new Vector2(ButtonWidth, ButtonHeight));
        menuButton.BackgroundSourceRect = buttonSourceRect;
        menuButton.OnClick = () => OnMenuRequested?.Invoke();
    }

    private static Texture2D CreateHamburgerIcon(GraphicsDevice graphicsDevice)
    {
        const int w = 30, h = 20;
        var data = new Color[w * h];

        int[] barStartYs = { 2, 8, 14 };
        const int barHeight = 4;

        foreach (int barY in barStartYs)
            for (int y = barY; y < barY + barHeight; y++)
                for (int x = 0; x < w; x++)
                    data[y * w + x] = Color.White;

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

    public void Update(GameTime gameTime)
    {
        var mode = buildManager.GetMode();
        cannonButton.SetSelected(mode == BuildMode.CannonTower);
        musketButton.SetSelected(mode == BuildMode.MusketTower);
        wallButton.SetSelected(mode == BuildMode.Wall);
        removeButton.SetSelected(mode == BuildMode.Remove);

        cannonButton.IsDisabled = progress.CoinsRemaining < TowerFactory.Cannon.Cost;
        musketButton.IsDisabled = progress.CoinsRemaining < TowerFactory.Musket.Cost;
        wallButton.IsDisabled = progress.CoinsRemaining < BuildManager.WallCost;

        cannonButton.Update(gameTime);
        musketButton.Update(gameTime);
        wallButton.Update(gameTime);
        removeButton.Update(gameTime);
        menuButton.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        healthBar.Draw(spriteBatch);

        var coinText = progress.CoinsRemaining.ToString();
        var coinSize = Primitives.Font.MeasureString(coinText);
        const int iconTextGap = 4;
        float totalWidth = coinIconSize + iconTextGap + coinSize.X;
        float coinRowX = panelX + (PanelWidth - totalWidth) / 2f;
        var iconRect = new Rectangle((int)coinRowX, coinAreaY, coinIconSize, coinIconSize);
        spriteBatch.Draw(coinIcon, iconRect, Color.White);
        float textY = coinAreaY + (coinIconSize - coinSize.Y) / 2f;
        spriteBatch.DrawString(Primitives.Font, coinText, new Vector2(coinRowX + coinIconSize + iconTextGap, textY), Color.Yellow);

        cannonButton.Draw(spriteBatch);
        musketButton.Draw(spriteBatch);
        wallButton.Draw(spriteBatch);
        removeButton.Draw(spriteBatch);
        menuButton.Draw(spriteBatch);
    }
}
