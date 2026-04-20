using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class Hud
{
    private SpriteFont font;


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

        buildMenu = new BuildMenu(buildManager, levelProgress);

        coinManager = new CoinManager(GetCoinTargetPosition, levelProgress);
        coinUIPosition = new Vector2(100, 50);
    }

    public Vector2 GetCoinTargetPosition()
    {
        var text = levelProgress.CoinsRemaining.ToString();
        var size = font.MeasureString(text);

        return coinUIPosition + size / 2f;
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

        spriteBatch.DrawString(
            font,
            levelProgress.CoinsRemaining.ToString(),
            coinUIPosition,
            Color.Yellow
        );
    }
}