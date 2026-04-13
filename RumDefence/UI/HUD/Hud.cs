using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class Hud
{
    private Texture2D panelTexture;
    private Texture2D buttonTexture;
    private SpriteFont font;

    private Rectangle panelRect;
    private SimpleButton wallButton;
    private SimpleButton musketButton;
    private SimpleButton cannonButton;

    private BuildManager buildManager;

    private CoinManager coinManager;

    private Vector2 coinUIPosition;

    public Hud(BuildManager buildManager)
    {
        this.buildManager = buildManager;

        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");

        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel_blue");
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button_blue");

        int panelWidth = 620;
        int panelHeight = 150;

        panelRect = new Rectangle(
            50,
            RumGame.VirtualHeight - panelHeight - 20,
            panelWidth,
            panelHeight
        );

        var buttonSize = new Vector2(180, 60);
        int buttonY = panelRect.Y + 45;

        wallButton = new SimpleButton(
            buttonTexture, font, "Wall",
            new Vector2(panelRect.X + 20, buttonY),
            buttonSize
        );
        wallButton.OnClick = () => buildManager.SetMode(BuildMode.Wall);

        musketButton = new SimpleButton(
            buttonTexture, font, "Musket",
            new Vector2(panelRect.X + 220, buttonY),
            buttonSize
        );
        musketButton.OnClick = () => buildManager.SetMode(BuildMode.MusketTower);

        cannonButton = new SimpleButton(
            buttonTexture, font, "Cannon",
            new Vector2(panelRect.X + 420, buttonY),
            buttonSize
        );
        cannonButton.OnClick = () => buildManager.SetMode(BuildMode.CannonTower);

        coinManager = new CoinManager(GetCoinTargetPosition);
        coinUIPosition = new Vector2(100, 50);
    }

    public Vector2 GetCoinTargetPosition()
    {
        var text = coinManager.Balance.ToString();
        var size = font.MeasureString(text);

        return coinUIPosition + size / 2f;
    }

    public CoinManager GetCoinManager()
    {
        return coinManager;
    }

    public void Update(GameTime gameTime)
    {
        wallButton.Update(gameTime);
        musketButton.Update(gameTime);
        cannonButton.Update(gameTime);
        coinManager.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(panelTexture, panelRect, Color.White);

        wallButton.Draw(spriteBatch);
        musketButton.Draw(spriteBatch);
        cannonButton.Draw(spriteBatch);

        coinManager.Draw(spriteBatch);

        spriteBatch.DrawString(
            font,
            coinManager.Balance.ToString(),
            coinUIPosition,
            Color.Yellow
        );
    }
}