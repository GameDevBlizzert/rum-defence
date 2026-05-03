using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class Hud
{
    private LevelProgressSystem levelProgress;
    private CoinManager coinManager;
    private BuildMenu buildMenu;
    private BuildManager buildManager;
    private WaveHud waveHud;
    private UpgradeMenu upgradeMenu;

    public Hud(BuildManager buildManager, LevelProgressSystem levelProgress, ShipSpawner spawner)
    {
        this.buildManager = buildManager;
        this.levelProgress = levelProgress;

        buildMenu = new BuildMenu(buildManager, levelProgress);
        coinManager = new CoinManager(buildMenu.GetCoinTargetPosition, levelProgress);
        waveHud = new WaveHud(spawner);
        
        upgradeMenu = new UpgradeMenu(levelProgress);
    }

    public CoinManager GetCoinManager()
    {
        return coinManager;
    }

    public void Update(GameTime gameTime)
    {
        buildMenu.Update(gameTime);
        coinManager.Update(gameTime);
        upgradeMenu.Update(gameTime);
    }
    
    public void SetSelectedTower(BaseTower tower)
    {
        upgradeMenu.SelectedTower = tower;
    }

    public bool WasUpgradeClicked()
    {
        return upgradeMenu.UpgradeClicked;
    }

    public bool IsMouseOverUpgradeMenu(Vector2 mousePos)
    {
        return upgradeMenu.SelectedTower != null && upgradeMenu.IsMouseOver(mousePos);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        buildMenu.Draw(spriteBatch);
        coinManager.Draw(spriteBatch);
        waveHud.Draw(spriteBatch);
        
        var hovered = buildManager.GetHoveredTile();
        // The game screen handles tracking current selection, we update the UpgradeMenu separately.
        upgradeMenu.Draw(spriteBatch);
    }
}
