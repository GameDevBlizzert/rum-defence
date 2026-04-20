using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class Hud
{
    private LevelProgressSystem levelProgress;
    private CoinManager coinManager;
    private BuildMenu buildMenu;
    private BuildManager buildManager;

    public Hud(BuildManager buildManager, LevelProgressSystem levelProgress)
    {
        this.buildManager = buildManager;
        this.levelProgress = levelProgress;

        buildMenu = new BuildMenu(buildManager, levelProgress);
        coinManager = new CoinManager(buildMenu.GetCoinTargetPosition, levelProgress);
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
    }
}
