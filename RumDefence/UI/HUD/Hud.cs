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

        int panelWidth = 400;
        int panelHeight = 150;

        panelRect = new Rectangle(
            50,
            RumGame.VirtualHeight - panelHeight - 20,
            panelWidth,
            panelHeight
        );

        wallButton = new SimpleButton(
            buttonTexture,
            font,
            "Wall",
            new Vector2(panelRect.X + 50, panelRect.Y + 40),
            new Vector2(300, 60)
        );

        wallButton.OnClick = () =>
        {
            buildManager.SetMode(BuildMode.Wall);
        };

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
        coinManager.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(panelTexture, panelRect, Color.White);

        wallButton.Draw(spriteBatch);

        coinManager.Draw(spriteBatch);

        spriteBatch.DrawString(
            font,
            coinManager.Balance.ToString(),
            coinUIPosition,
            Color.Yellow
        );
    }
}