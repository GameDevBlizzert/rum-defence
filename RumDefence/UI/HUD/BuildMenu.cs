using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class BuildMenu
{
    private Texture2D panelTexture;
    private Texture2D pixelTexture;

    private Rectangle panelRect;

    private IconButton wallButton;
    private IconButton cannonButton;
    private IconButton musketButton;
    private IconButton removeButton;

    private BuildManager buildManager;

    private const int PanelWidth = 120;
    private const int ButtonSize = 80;
    private const int ButtonMargin = 14;
    private const int HeaderHeight = 60;
    private const int LabelHeight = 28;

    public BuildMenu(BuildManager buildManager)
    {
        this.buildManager = buildManager;

        var content = RumGame.Instance.Content;
        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel_blue");

        var wallIcon = content.Load<Texture2D>("Art/Themes/Grass/Walls/wall");
        var cannonIcon = content.Load<Texture2D>("KenneyPiratePack/PNG/Default size/Ship parts/cannon");
        var removeIcon = content.Load<Texture2D>("KenneyUIPack/PNG/Blue/Default/icon_cross");

        pixelTexture = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
        pixelTexture.SetData(new[] { Color.White });

        int panelX = 20;
        int panelY = 20;
        int panelHeight = RumGame.VirtualHeight - 40;

        panelRect = new Rectangle(panelX, panelY, PanelWidth, panelHeight);

        int buttonX = panelX + (PanelWidth - ButtonSize) / 2;
        int sectionStart = panelY + HeaderHeight + LabelHeight + 8;

        wallButton = new IconButton(
            panelTexture,
            wallIcon,
            new Vector2(buttonX, sectionStart),
            new Vector2(ButtonSize, ButtonSize)
        );
        wallButton.OnClick = () => buildManager.SetMode(BuildMode.Wall);

        int cannonY = sectionStart + ButtonSize + ButtonMargin + LabelHeight + 8;

        cannonButton = new IconButton(
            panelTexture,
            cannonIcon,
            new Vector2(buttonX, cannonY),
            new Vector2(ButtonSize, ButtonSize)
        );
        cannonButton.OnClick = () => buildManager.SetMode(BuildMode.CannonTower);
        int musketY = cannonY + ButtonSize + ButtonMargin + LabelHeight + 8;

        musketButton = new IconButton(
                  panelTexture,
                  cannonIcon,
                  new Vector2(buttonX, musketY),
                  new Vector2(ButtonSize, ButtonSize)
              );
        musketButton.OnClick = () => buildManager.SetMode(BuildMode.MusketTower);

        int removeY = musketY + ButtonSize + ButtonMargin + LabelHeight + 8;

        removeButton = new IconButton(
            panelTexture,
            removeIcon,
            new Vector2(buttonX, removeY),
            new Vector2(ButtonSize, ButtonSize)
        );
        removeButton.BaseTint = new Color(220, 70, 70);
        removeButton.OnClick = () => buildManager.SetMode(BuildMode.Remove);
    }

    public void Update(GameTime gameTime)
    {
        var mode = buildManager.GetMode();
        wallButton.SetSelected(mode == BuildMode.Wall);
        cannonButton.SetSelected(mode == BuildMode.CannonTower);
        musketButton.SetSelected(mode == BuildMode.MusketTower);
        removeButton.SetSelected(mode == BuildMode.Remove);

        wallButton.Update(gameTime);
        cannonButton.Update(gameTime);
        musketButton.Update(gameTime);
        removeButton.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Panel background
        spriteBatch.Draw(panelTexture, panelRect, Color.White);

        // Section labels + buttons
        wallButton.Draw(spriteBatch);

        cannonButton.Draw(spriteBatch);

        musketButton.Draw(spriteBatch);

        removeButton.Draw(spriteBatch);
    }
}
