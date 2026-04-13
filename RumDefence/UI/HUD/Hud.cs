using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class Hud
{
    private SpriteFont font;

    private BuildMenu buildMenu;

    private CoinManager coinManager;

    private Vector2 coinUIPosition;

    public Hud(BuildManager buildManager)
    {
        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");

        buildMenu = new BuildMenu(buildManager);

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
        buildMenu.Update(gameTime);
        coinManager.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        buildMenu.Draw(spriteBatch);

        coinManager.Draw(spriteBatch);

        spriteBatch.DrawString(
            font,
            coinManager.Balance.ToString(),
            coinUIPosition,
            Color.Yellow
        );
    }
}