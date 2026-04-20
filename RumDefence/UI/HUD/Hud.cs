using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class Hud
{
    private SpriteFont font;
    private Texture2D coinIcon;
    private Texture2D pixelTexture;

    private LevelProgressSystem levelProgress;

    private CoinManager coinManager;

    private Vector2 coinUIPosition;
    private BuildMenu buildMenu;
    private BuildManager buildManager;

    public Hud(BuildManager buildManager, LevelProgressSystem levelProgress)
    {
        this.buildManager = buildManager;
        this.levelProgress = levelProgress;

        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");
        coinIcon = content.Load<Texture2D>("Art/UI/Coin");

        // Create pixel texture for background
        pixelTexture = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
        pixelTexture.SetData(new[] { Color.White });

        buildMenu = new BuildMenu(buildManager, levelProgress);

        coinManager = new CoinManager(GetCoinTargetPosition, levelProgress);

        // Position coin display centered within build menu
        // Build menu: x=20, width=200
        var coinText = levelProgress.CoinsRemaining.ToString();
        var textSize = font.MeasureString(coinText);
        var totalWidth = 32 + 8 + textSize.X;
        var centerX = 20 + (200 - totalWidth) / 2;
        coinUIPosition = new Vector2(centerX, 35);
    }

    public Vector2 GetCoinTargetPosition()
    {
        var text = levelProgress.CoinsRemaining.ToString();
        var textSize = font.MeasureString(text);

        // Total width: coin (32) + gap (8) + text
        var totalWidth = 32 + 8 + textSize.X;
        var centerOffset = totalWidth / 2f;

        // Position at center of combined coin + balance
        return coinUIPosition + new Vector2(centerOffset, 16);
    }

    public CoinManager GetCoinManager()
    {
        return coinManager;
    }

    public void Update(GameTime gameTime)
    {
        buildMenu.Update(gameTime);
        coinManager.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        buildMenu.Draw(spriteBatch);

        coinManager.Draw(spriteBatch);

        // Draw coin icon
        var coinRect = new Rectangle((int)coinUIPosition.X, (int)coinUIPosition.Y, 32, 32);
        spriteBatch.Draw(coinIcon, coinRect, Color.White);

        // Draw coin balance text
        var coinText = levelProgress.CoinsRemaining.ToString();
        var textX = coinUIPosition.X + 40;
        var textSize = font.MeasureString(coinText);
        var textY = coinUIPosition.Y + (32 - textSize.Y) / 2;

        spriteBatch.DrawString(
            font,
            coinText,
            new Vector2(textX, textY),
            Color.White
        );
    }
}
