using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RumDefence;

public class GameScreen : Screen
{
    private Grid grid { get; set; }
    private GridRenderer renderer { get; set; }
    private Level currentLevel { get; set; }
    private BuildManager buildManager { get; set; }
    private InputManager input { get; set; }
    private Hud hud { get; set; }

    private Dictionary<Point, Wall> walls = new();
    private WallRenderer wallRenderer;

    public ShipSpawner Spawner { get; private set; }

    public List<Ship> Ships { get; private set; } = new();
    public List<Troop> Troops { get; private set; } = new();

    private Dictionary<Point, BaseTower> placedTowers = new();
    private List<Explosion> explosions = new();

    private bool levelCompleted;

    private LevelProgressSystem progress;

    private HashSet<Point> latestUntraverableHashSet = new();

    public GameScreen(ScreenManager manager, Level level) : base(manager)
    {
        currentLevel = level;
    }

    public override void Load()
    {
        grid = new Grid(currentLevel.Map);

        RumGame.Instance.CurrentGrid = grid;
        RumGame.Instance.CurrentLevel = currentLevel;

        GridSystem.CalculateLayout(grid);

        input = new InputManager();
        buildManager = new BuildManager(grid);

        renderer = new GridRenderer(currentLevel.Theme.Tiles, buildManager, grid);

        progress = new(currentLevel.StartingLives, currentLevel.StartingCoinBalance);
        currentLevel.RumBarrel.OnDamageTaken = amount => progress.TakeHits(amount);

        Spawner = new ShipSpawner(currentLevel, grid);

        hud = new Hud(buildManager, progress, Spawner);

        wallRenderer = new WallRenderer(
            grid,
            currentLevel.Theme.Walls,
            walls
        );

        var occupiedTiles = new Dictionary<Point, bool>();
        renderer.SetOccupiedTiles(occupiedTiles);

        buildManager.SetWallPlacementCallback(p =>
        {
            if (!walls.ContainsKey(p) && !placedTowers.ContainsKey(p) && progress.CoinsRemaining >= BuildManager.WallCost)
            {
                walls[p] = new Wall(p);
                occupiedTiles[p] = true;
                progress.SpendCoins(BuildManager.WallCost);
                // Play random impact sound when wall is placed
                AudioManager.Instance.PlayRandomImpact();
            }
        });

        buildManager.SetRemoveCallback(p =>
        {
            int refundAmount = 0;

            if (walls.ContainsKey(p))
            {
                refundAmount = (int)Math.Ceiling(BuildManager.WallCost * 0.8f);
                walls.Remove(p);
            }
            else if (placedTowers.TryGetValue(p, out BaseTower tower))
            {
                if (tower is CannonTower)
                    refundAmount = (int)Math.Ceiling(BuildManager.CannonTowerCost * 0.8f);
                else if (tower is MusketTower)
                    refundAmount = (int)Math.Ceiling(BuildManager.MusketTowerCost * 0.8f);
                
                placedTowers.Remove(p);
            }

            if (refundAmount > 0)
            {
                occupiedTiles.Remove(p);
                progress.AddCoins(refundAmount);
                AudioManager.Instance.PlayRandomImpact();
            }

        });

        buildManager.SetCannonTowerPlacementCallback(p =>
        {
            if (!placedTowers.ContainsKey(p) && !walls.ContainsKey(p) && progress.CoinsRemaining >= BuildManager.CannonTowerCost)
            {
                var cannon = new CannonTower(grid.GridToWorld(p), Troops);
                cannon.SetProjectileHitCallback((pos, explosionIndex) =>
                {
                    explosions.Add(new Explosion(pos, explosionIndex));
                });
                placedTowers[p] = cannon;
                occupiedTiles[p] = true;
                progress.SpendCoins(BuildManager.CannonTowerCost);
                AudioManager.Instance.PlayRandomImpact();
            }
        });

        buildManager.SetMusketTowerPlacementCallback(p =>
        {
            if (!placedTowers.ContainsKey(p) && !walls.ContainsKey(p) && progress.CoinsRemaining >= BuildManager.MusketTowerCost)
            {
                placedTowers[p] = new MusketTower(grid.GridToWorld(p), Troops);
                occupiedTiles[p] = true;
                progress.SpendCoins(BuildManager.MusketTowerCost);
                AudioManager.Instance.PlayRandomImpact();
            }
        });

        AudioManager.Instance.PlayBackgroundMusic();
    }

    public override void Update(GameTime gameTime)
    {
        if (HandlePause()) return;
        input.Update();
        UpdateBuildSystem(gameTime);
        UpdateSpawner(gameTime);
        UpdateShips(gameTime);
        UpdateTroops(gameTime);
        UpdateTowers(gameTime);
        UpdateWaveProgression();
        CheckLevelCompletion(gameTime);
    }

    private void UpdateTowers(GameTime gameTime)
    {
        // Update all towers
        foreach (var tower in placedTowers.Values)
            tower.Update(gameTime);

        // Update and remove finished explosions
        for (int i = explosions.Count - 1; i >= 0; i--)
        {
            explosions[i].Update(gameTime);
            if (explosions[i].IsFinished)
                explosions.RemoveAt(i);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(new Color(30, 144, 255));

        renderer.Draw(grid, spriteBatch);

        wallRenderer.Draw(spriteBatch);

        foreach (var ship in Ships)
            ship.Draw(spriteBatch);

        foreach (var troop in Troops)
            troop.Draw(spriteBatch);

        foreach (var tower in placedTowers.Values)
            tower.Draw(spriteBatch);

        // Draw explosions
        foreach (var explosion in explosions)
            explosion.Draw(spriteBatch);

        var overlayRenderer = renderer.GetOverlayRenderer();
        if (overlayRenderer != null)
        {
            overlayRenderer.Draw(spriteBatch);
        }

        hud.Draw(spriteBatch);
    }

    private void UnlockNextLevel()
    {
        int currentIndex = GrassLevels.All.IndexOf(currentLevel);

        if (currentIndex + 1 < GrassLevels.All.Count)
        {
            GrassLevels.All[currentIndex + 1].IsUnlocked = true;
        }
    }

    private void UpdateBuildSystem(GameTime gameTime)
    {
        hud.Update(gameTime);

        buildManager.Update(
            input.MousePositionScaled,
            input.IsLeftClick()
        );
    }

    private bool HandlePause()
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Escape))
        {
            manager.SetScreen(new PauseScreen(manager, this));
            return true;
        }

        return false;
    }

    private void UpdateSpawner(GameTime gameTime)
    {
        Spawner.Update(gameTime);
        Ships.AddRange(Spawner.NewShips);
    }

    private void UpdateShips(GameTime gameTime)
    {
        for (int i = Ships.Count - 1; i >= 0; i--)
        {
            Ships[i].Update(gameTime);

            if (Ships[i].SpawnedTroops.Count > 0)
            {
                Troops.AddRange(Ships[i].SpawnedTroops);
                Ships[i].SpawnedTroops.Clear();
            }

            if (Ships[i].IsFinished)
            {
                Ships.RemoveAt(i);
            }
        }
    }

    private void UpdateTroops(GameTime gameTime)
    {
        var untraversable = GetUntraversableTiles();
        var updatePaths = !latestUntraverableHashSet.SetEquals(untraversable);

        latestUntraverableHashSet = untraversable;

        if (updatePaths)
            grid.UntraversableTiles = latestUntraverableHashSet;

        for (int i = Troops.Count - 1; i >= 0; i--)
        {
            var troop = Troops[i];
            troop.Update(gameTime);

            if (updatePaths || troop.NeedsPathInit)
                troop.UpdatePathfinding();

            if (troop.IsDead && !troop.HasDroppedReward)
            {
                hud.GetCoinManager().SpawnCoin(troop.Position, troop.CoinValue);
                troop.MarkRewardGiven();
            }

            if (troop.IsFinished || troop.IsDead)
            {
                if (troop.IsFinished)
                    progress.TakeHits(1);

                Troops.RemoveAt(i);
            }
        }
    }

    private void UpdateWaveProgression()
    {
        if (Spawner.IsAllWavesComplete) return;
        if (!Spawner.HasPreloadedWave) return;

        bool attackingShipsRemain = Ships.Any(s =>
            s.State == Ship.ShipState.SailingToDock ||
            s.State == Ship.ShipState.Docked ||
            s.State == Ship.ShipState.Unloading ||
            s.State == Ship.ShipState.Leaving_BackOff ||
            s.State == Ship.ShipState.Leaving_ToSea);

        if (!attackingShipsRemain && Troops.Count == 0)
            Spawner.AdvancePreloadToAttack();
    }

    private void CheckLevelCompletion(GameTime gameTime)
    {
        if (!levelCompleted && Spawner.IsAllWavesComplete && Ships.Count == 0 && Troops.Count == 0)
            progress.SetWon();

        // TODO: Do not ignore IsLost after testing
        levelCompleted = progress.IsWon() /*|| progress.IsLost()*/;

        if (levelCompleted)
        {
            UnlockNextLevel();
            AudioManager.Instance.StopBackgroundMusic();
            // TODO: Show win or lose screen based
            manager.SetScreen(new MainMenuScreen(manager));
        }
    }

    /// <summary>
    /// Get all the tiles troops cannot traverse, including walls and water tiles. Used for pathfinding.
    /// </summary>
    /// <returns>List of untraversable grid tiles</returns>
    private HashSet<Point> GetUntraversableTiles()
    {
        var untraversable = new HashSet<Point>();

        foreach (var wall in walls.Values)
        {
            untraversable.Add(wall.GridPos);
        }

        //foreach (var tower in placedTowers)
        //{
        //    var tile = grid.WorldToGrid(tower.);
        //    if (tile != null)
        //        untraversable.Add(tile.Value);
        //}

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (TileRules.IsWater(grid.Tiles[y, x]))
                {
                    untraversable.Add(new Point(x, y));
                }
            }
        }

        return untraversable;
    }

}
