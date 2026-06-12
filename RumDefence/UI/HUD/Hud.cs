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
    private WallRepairMenu wallRepairMenu;

    public Hud(BuildManager buildManager, LevelProgressSystem levelProgress, ShipSpawner spawner, System.Func<TowerData, bool> isTowerAvailable = null)
    {
        this.buildManager = buildManager;
        this.levelProgress = levelProgress;

        buildMenu = new BuildMenu(buildManager, levelProgress, isTowerAvailable);
        coinManager = new CoinManager(buildMenu.GetCoinTargetPosition, levelProgress);
        waveHud = new WaveHud(spawner);

        upgradeMenu = new UpgradeMenu(levelProgress);
        wallRepairMenu = new WallRepairMenu(levelProgress);
    }

    public System.Action OnSpeedRequested
    {
        set => buildMenu.OnSpeedRequested = value;
    }

    public System.Action OnMenuRequested
    {
        set => buildMenu.OnMenuRequested = value;
    }

    public TowerType? FreeTowerType
    {
        set => buildMenu.FreeTowerType = value;
    }

    public TowerType? HighlightedTower
    {
        set => buildMenu.HighlightedTower = value;
    }

    public void RefreshBuildMenu() => buildMenu.Refresh();

    public void SetPlaybackState(GamePlaybackState playbackState)
    {
        buildMenu.SetPlaybackState(playbackState);
        upgradeMenu.IsDisabled = playbackState == GamePlaybackState.Paused;
        wallRepairMenu.IsDisabled = playbackState == GamePlaybackState.Paused;
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
        wallRepairMenu.Update(gameTime);
    }

    public void SetSelectedTower(BaseTower tower)
    {
        upgradeMenu.SelectedTower = tower;
    }

    public void SetSelectedWall(Wall wall)
    {
        wallRepairMenu.SelectedWall = wall;
        if (wall != null)
            upgradeMenu.SelectedTower = null;
    }

    public bool WasUpgradeClicked()
    {
        return upgradeMenu.UpgradeClicked;
    }

    public bool WasTargetModeClicked()
    {
        return upgradeMenu.TargetModeClicked;
    }

    public bool IsMouseOverUpgradeMenu(Vector2 mousePos)
    {
        bool upgradeVisible = upgradeMenu.SelectedTower != null && upgradeMenu.IsMouseOver(mousePos);
        bool repairVisible = wallRepairMenu.SelectedWall != null && wallRepairMenu.IsMouseOver(mousePos);
        return upgradeVisible || repairVisible;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        buildMenu.Draw(spriteBatch);
        coinManager.Draw(spriteBatch);
        waveHud.Draw(spriteBatch);

        upgradeMenu.Draw(spriteBatch);
        wallRepairMenu.Draw(spriteBatch);
    }

    public bool WasRepairClicked()
    {
        return wallRepairMenu.RepairClicked;
    }

    public bool WasWallUpgradeClicked()
    {
        return wallRepairMenu.UpgradeClicked;
    }
}
