using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class BuildMenu
{
    private Texture2D pixelTexture;
    private FantasyPanel panelBorder;
    private FantasyPanel panelBackground;

    private Rectangle panelRect;

    private IconButton wallButton;
    private IconButton cannonButton;
    private IconButton musketButton;
    private IconButton removeButton;

    private BuildManager buildManager;
    private LevelProgressSystem progress;

    private const int PanelWidth = 200;
    private const int ButtonSize = 80;
    private const int ButtonMargin = 14;
    private const int HeaderHeight = 60;
    private const int LabelHeight = 28;

    public BuildMenu(BuildManager buildManager, LevelProgressSystem progress)
    {
        this.buildManager = buildManager;
        this.progress = progress;

        var content = RumGame.Instance.Content;

        var wallIcon = content.Load<Texture2D>("Art/Themes/Grass/Walls/wall");
        var cannonIcon = content.Load<Texture2D>("KenneyPiratePack/PNG/Default size/Ship parts/cannon");
        var removeIcon = content.Load<Texture2D>("KenneyUIPack/PNG/Blue/Default/icon_cross");

        pixelTexture = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
        pixelTexture.SetData(new[] { Color.White });

        int panelX = 20;
        int panelY = 20;
        int panelHeight = RumGame.VirtualHeight - 40;

        panelRect = new Rectangle(panelX, panelY, PanelWidth, panelHeight);
        
        // Background: draw full panel in black (corners, edges, and center)
        panelBackground = new FantasyPanel(0);
        panelBackground.SetBounds(panelRect);
        panelBackground.DrawCenterFill = true;
        panelBackground.Tint = Color.Black;
        
        // Border: draw only the border outline in white (no center fill)
        panelBorder = new FantasyPanel(0);
        panelBorder.SetBounds(panelRect);
        panelBorder.DrawCenterFill = false;
        panelBorder.Tint = Color.White;

        int buttonX = panelX + (PanelWidth - ButtonSize) / 2;
        int sectionStart = panelY + HeaderHeight + LabelHeight + 8;

        wallButton = new IconButton(
            wallIcon,
            new Vector2(buttonX, sectionStart),
            new Vector2(ButtonSize, ButtonSize)
        );
        wallButton.OnClick = () => buildManager.SetMode(BuildMode.Wall);

        int cannonY = sectionStart + ButtonSize + ButtonMargin + LabelHeight + 8;

        cannonButton = new IconButton(
            cannonIcon,
            new Vector2(buttonX, cannonY),
            new Vector2(ButtonSize, ButtonSize)
        );
        cannonButton.OnClick = () => buildManager.SetMode(BuildMode.CannonTower);
        int musketY = cannonY + ButtonSize + ButtonMargin + LabelHeight + 8;

        musketButton = new IconButton(
                  cannonIcon,
                  new Vector2(buttonX, musketY),
                  new Vector2(ButtonSize, ButtonSize)
              );
        musketButton.OnClick = () => buildManager.SetMode(BuildMode.MusketTower);

        int removeY = musketY + ButtonSize + ButtonMargin + LabelHeight + 8;

        removeButton = new IconButton(
            removeIcon,
            new Vector2(buttonX, removeY),
            new Vector2(ButtonSize, ButtonSize)
        );
        removeButton.OnClick = () => buildManager.SetMode(BuildMode.Remove);
    }

    public void Update(GameTime gameTime)
    {
        var mode = buildManager.GetMode();
        wallButton.SetSelected(mode == BuildMode.Wall);
        cannonButton.SetSelected(mode == BuildMode.CannonTower);
        musketButton.SetSelected(mode == BuildMode.MusketTower);
        removeButton.SetSelected(mode == BuildMode.Remove);

        wallButton.IsDisabled = progress.CoinsRemaining < BuildManager.WallCost;
        cannonButton.IsDisabled = progress.CoinsRemaining < BuildManager.CannonTowerCost;
        musketButton.IsDisabled = progress.CoinsRemaining < BuildManager.MusketTowerCost;

        wallButton.Update(gameTime);
        cannonButton.Update(gameTime);
        musketButton.Update(gameTime);
        removeButton.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Panel background (black) then border (white)
        panelBackground.Draw(spriteBatch);
        panelBorder.Draw(spriteBatch);

        // Section labels + buttons
        wallButton.Draw(spriteBatch);

        cannonButton.Draw(spriteBatch);

        musketButton.Draw(spriteBatch);

        removeButton.Draw(spriteBatch);
    }
}
