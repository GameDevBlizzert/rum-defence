using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

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

    private BaseTower selectedTower = null;

    private bool levelCompleted;

    private LevelProgressSystem progress;

    private HashSet<Point> latestUntraverableHashSet = new();

    private Texture2D pixel;

    public GameScreen(ScreenManager manager, Level level) : base(manager)
    {
        currentLevel = level;
    }

    public override void Load()
    {
        pixel = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

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
        hud.OnMenuRequested = () => manager.SetScreen(new PauseScreen(manager, this));

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
                    refundAmount = (int)Math.Ceiling(TowerFactory.Cannon.Cost * 0.8f);
                else if (tower is MusketTower)
                    refundAmount = (int)Math.Ceiling(TowerFactory.Musket.Cost * 0.8f);

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
            if (!placedTowers.ContainsKey(p) && !walls.ContainsKey(p) && progress.CoinsRemaining >= TowerFactory.Cannon.Cost)
            {
                placedTowers[p] = TowerFactory.Create(
                    TowerFactory.Cannon,
                    grid.GridToWorld(p),
                    Troops,
                    (pos, explosionIndex) => explosions.Add(new Explosion(pos, explosionIndex))
                );
                occupiedTiles[p] = true;
                progress.SpendCoins(TowerFactory.Cannon.Cost);
                AudioManager.Instance.PlayRandomImpact();
                // Select newly placed tower
                selectedTower = placedTowers[p];
                buildManager.SetMode(BuildMode.None); // Auto select without button
            }
        });

        buildManager.SetMusketTowerPlacementCallback(p =>
        {
            if (!placedTowers.ContainsKey(p) && !walls.ContainsKey(p) && progress.CoinsRemaining >= TowerFactory.Musket.Cost)
            {
                placedTowers[p] = TowerFactory.Create(TowerFactory.Musket, grid.GridToWorld(p), Troops);
                occupiedTiles[p] = true;
                progress.SpendCoins(TowerFactory.Musket.Cost);
                AudioManager.Instance.PlayRandomImpact();
                // Select newly placed tower
                selectedTower = placedTowers[p];
                buildManager.SetMode(BuildMode.None); // Auto select without button
            }
        });

        buildManager.SetSelectCallback(p =>
        {
            if (placedTowers.TryGetValue(p, out BaseTower tower))
            {
                selectedTower = tower;
            }
            else
            {
                selectedTower = null;
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

        // Draw range of selected tower
        if (selectedTower != null)
        {
            DrawCircle(spriteBatch, selectedTower.Position, selectedTower.CurrentRange, Color.White * 0.2f);
        }

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
        // If mode is reset to something else (e.g. wall building), clear selection
        if (buildManager.GetMode() != BuildMode.None)
        {
            selectedTower = null;
        }

        hud.SetSelectedTower(selectedTower);

        hud.Update(gameTime);

        // Handle HUD Upgrade interaction
        if (selectedTower != null && hud.WasUpgradeClicked())
        {
            if (progress.CoinsRemaining >= selectedTower.GetUpgradeCost() && selectedTower.CanUpgrade)
            {
                progress.SpendCoins(selectedTower.GetUpgradeCost());
                selectedTower.ApplyUpgrade();
                AudioManager.Instance.PlayRandomImpact();
            }
        }

        // Only process tile clicks if the mouse is NOT over the upgrade menu
        if (!hud.IsMouseOverUpgradeMenu(input.MousePositionScaled))
        {
            buildManager.Update(
                input.MousePositionScaled,
                input.IsLeftClick()
            );
        }
        else
        {
            // Still update the hovering logic so visuals don't freeze, but consume the click
            buildManager.Update(
                input.MousePositionScaled,
                false // forcefully tell buildManager it's NOT a click because the UI consumed it
            );
        }
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

                if (troop.CanBeRemoved)
                    Troops.RemoveAt(i);
            }
        }
    }

    private void CheckLevelCompletion(GameTime gameTime)
    {
        // Lose condition
        if (progress.IsLost())
        {
            AudioManager.Instance.StopBackgroundMusic();
            manager.SetScreen(new GameOverScreen(
                manager,
                this,
                currentLevel,
                false,
                Spawner.CurrentWave,
                progress.CoinsRemaining
            ));
            return;
        }

        progress.Update(gameTime, this);

        // Win condition
        if (!levelCompleted && Spawner.IsAllWavesComplete && Ships.Count == 0 && Troops.Count == 0)
            progress.SetWon();

        if (!levelCompleted && progress.IsWon())
        {
            levelCompleted = true;

            UnlockNextLevel();
            AudioManager.Instance.StopBackgroundMusic();

            manager.SetScreen(new GameOverScreen(
                manager,
                this,
                currentLevel,
                true,
                Spawner.CurrentWave,
                progress.CoinsRemaining
            ));
            return;
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

    private void DrawCircle(SpriteBatch spriteBatch, Vector2 center, float radius, Color color)
    {
        int points = 32;
        float step = MathHelper.TwoPi / points;

        for (int i = 0; i < points; i++)
        {
            float angle1 = i * step;
            float angle2 = (i + 1) * step;

            Vector2 p1 = new Vector2((float)Math.Cos(angle1), (float)Math.Sin(angle1)) * radius;
            Vector2 p2 = new Vector2((float)Math.Cos(angle2), (float)Math.Sin(angle2)) * radius;

            DrawLine(spriteBatch, center + p1, center + p2, color, 2);
        }
    }

    private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
    {
        Vector2 edge = end - start;
        float angle = (float)Math.Atan2(edge.Y, edge.X);

        spriteBatch.Draw(pixel,
            new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), (int)thickness),
            null,
            color,
            angle,
            new Vector2(0, 0.5f),
            SpriteEffects.None,
            0);
    }
}
