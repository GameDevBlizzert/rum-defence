using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace RumDefence;

public enum GamePlaybackState
{
    Normal,
    FastForward,
    Paused
}

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
    public static GameScreen Instance { get; private set; }
    public List<Explosion> Explosions = new();
    public List<NetEffect> NetEffects = new();
    public List<FireEffect> FireEffects = new();

    private Dictionary<Point, BaseTower> placedTowers = new();

    private BaseTower selectedTower = null;

    private bool levelCompleted;

    private LevelProgressSystem progress;

    private HashSet<Point> latestUntraverableHashSet = new();

    private Dictionary<Point, bool> occupiedTiles = new();

    private GamePlaybackState playbackState = GamePlaybackState.Normal;
    public List<Level> ActiveLevelSet { get; private set; }
    public Level CurrentLevel => currentLevel;

    private TutorialOverlay tutorialOverlay;
    private bool tutorialWaveNotified = false;

    public GameScreen(ScreenManager manager, Level level, List<Level> levelSet) : base(manager)
    {
        currentLevel = level;
        ActiveLevelSet = levelSet;
        Instance = this;
    }

    public override void Load()
    {
        grid = new Grid(currentLevel.Map);

        RumGame.Instance.CurrentGrid = grid;
        RumGame.Instance.CurrentLevel = currentLevel;

        GridSystem.CalculateLayout(grid);

        input = InputManager.Instance;
        buildManager = new BuildManager(grid, currentLevel.RumTile);

        renderer = new GridRenderer(currentLevel.Theme.Tiles, buildManager, grid);

        progress = new(currentLevel.StartingLives, currentLevel.StartingCoinBalance, ActiveLevelSet, currentLevel);
        currentLevel.RumBarrel.OnDamageTaken = amount => progress.TakeHits(amount);

        Spawner = new ShipSpawner(currentLevel, grid);

        hud = new Hud(buildManager, progress, Spawner);
        hud.SetPlaybackState(playbackState);
        hud.OnSpeedRequested = CyclePlaybackState;
        hud.OnMenuRequested = () => manager.SetScreen(new PauseScreen(manager, this));

        wallRenderer = new WallRenderer(
            grid,
            currentLevel.Theme.Walls,
            walls
        );

        renderer.SetOccupiedTiles(occupiedTiles);

        buildManager.SetWallPlacementCallback(p =>
        {
            if (!walls.ContainsKey(p) && !placedTowers.ContainsKey(p) &&
                progress.CoinsRemaining >= BuildManager.WallCost)
            {
                currentLevel.Decorations.RemoveAll(d => d.GridPos == p);
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
                else if (tower is FireTower)
                    refundAmount = (int)Math.Ceiling(TowerFactory.Fire.Cost * 0.8f);

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
            if (!placedTowers.ContainsKey(p) && !walls.ContainsKey(p) &&
                progress.CoinsRemaining >= TowerFactory.Cannon.Cost)
            {
                currentLevel.Decorations.RemoveAll(d => d.GridPos == p);
                placedTowers[p] = TowerFactory.Create(
                    TowerFactory.Cannon,
                    grid.GridToWorld(p),
                    Troops
                );
                occupiedTiles[p] = true;
                progress.SpendCoins(TowerFactory.Cannon.Cost);
                AudioManager.Instance.PlayRandomImpact();
                if (!buildManager.CtrlHeld)
                {
                    selectedTower = placedTowers[p];
                    buildManager.SetMode(BuildMode.None);
                }
            }
        });

        buildManager.SetMusketTowerPlacementCallback(p =>
        {
            if (!placedTowers.ContainsKey(p) && !walls.ContainsKey(p) &&
                progress.CoinsRemaining >= TowerFactory.Musket.Cost)
            {
                currentLevel.Decorations.RemoveAll(d => d.GridPos == p);
                placedTowers[p] = TowerFactory.Create(TowerFactory.Musket, grid.GridToWorld(p), Troops);
                occupiedTiles[p] = true;
                progress.SpendCoins(TowerFactory.Musket.Cost);
                AudioManager.Instance.PlayRandomImpact();
                if (!buildManager.CtrlHeld)
                {
                    selectedTower = placedTowers[p];
                    buildManager.SetMode(BuildMode.None);
                }
            }
        });

        buildManager.SetFisherTowerPlacementCallback(p =>
        {
            if (!placedTowers.ContainsKey(p) && !walls.ContainsKey(p) &&
                progress.CoinsRemaining >= TowerFactory.Fisher.Cost)
            {
                currentLevel.Decorations.RemoveAll(d => d.GridPos == p);
                placedTowers[p] = TowerFactory.Create(TowerFactory.Fisher, grid.GridToWorld(p), Troops);
                occupiedTiles[p] = true;
                progress.SpendCoins(TowerFactory.Fisher.Cost);
                AudioManager.Instance.PlayRandomImpact();
                // Select newly placed tower
                selectedTower = placedTowers[p];
                buildManager.SetMode(BuildMode.None); // Auto select without button
            }
        });

        buildManager.SetFireTowerPlacementCallback(p =>
        {
            if (!placedTowers.ContainsKey(p) && !walls.ContainsKey(p) &&
                progress.CoinsRemaining >= TowerFactory.Fire.Cost)
            {
                currentLevel.Decorations.RemoveAll(d => d.GridPos == p);
                placedTowers[p] = TowerFactory.Create(TowerFactory.Fire, grid.GridToWorld(p), Troops);
                occupiedTiles[p] = true;
                progress.SpendCoins(TowerFactory.Fire.Cost);
                AudioManager.Instance.PlayRandomImpact();
                if (!buildManager.CtrlHeld)
                {
                    selectedTower = placedTowers[p];
                    buildManager.SetMode(BuildMode.None);
                }
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

        if (currentLevel.Id == 1)
            tutorialOverlay = new TutorialOverlay();

        AudioManager.Instance.PlayBackgroundMusic();
    }

    public override void Update(GameTime gameTime)
    {
        if (HandlePause()) return;

        UpdateBuildSystem(gameTime);

        if (playbackState == GamePlaybackState.Paused)
            return;

        var gameplayGameTime = GetGameplayGameTime(gameTime);

        UpdateSpawner(gameplayGameTime);
        UpdateShips(gameplayGameTime);
        UpdateTroops(gameplayGameTime);
        UpdateWalls();
        UpdateTowers(gameplayGameTime);
        CheckLevelCompletion(gameplayGameTime);
    }

    private void UpdateTowers(GameTime gameTime)
    {
        // Update all towers
        foreach (var tower in placedTowers.Values)
            tower.Update(gameTime);

        // Update and remove finished explosions
        for (int i = Explosions.Count - 1; i >= 0; i--)
        {
            Explosions[i].Update(gameTime);
            if (Explosions[i].IsFinished)
                Explosions.RemoveAt(i);
        }

        for (int i = NetEffects.Count - 1; i >= 0; i--)
        {
            NetEffects[i].Update(gameTime);
            if (NetEffects[i].IsFinished)
                NetEffects.RemoveAt(i);
        }

        for (int i = FireEffects.Count - 1; i >= 0; i--)
        {
            FireEffects[i].Update(gameTime);
            if (FireEffects[i].IsFinished)
                FireEffects.RemoveAt(i);
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

        foreach (var deco in currentLevel.Decorations)
            deco.Draw(spriteBatch, grid);

        // Draw range of selected tower
        if (selectedTower != null)
        {
            DrawCircle(spriteBatch, selectedTower.Position, selectedTower.CurrentRange, Color.White * 0.2f);
        }

        // Draw explosions
        foreach (var explosion in Explosions)
            explosion.Draw(spriteBatch);

        foreach (var net in NetEffects)
            net.Draw(spriteBatch);

        foreach (var fire in FireEffects)
            fire.Draw(spriteBatch);

        DrawWallHealthBars(spriteBatch);

        var overlayRenderer = renderer.GetOverlayRenderer();
        if (overlayRenderer != null)
        {
            overlayRenderer.Draw(spriteBatch);
        }

        hud.Draw(spriteBatch);
        tutorialOverlay?.Draw(spriteBatch);
    }

    private void UpdateBuildSystem(GameTime gameTime)
    {
        // If mode is reset to something else (e.g. wall building), clear selection
        if (buildManager.GetMode() != BuildMode.None)
        {
            selectedTower = null;
        }

        hud.SetSelectedTower(selectedTower);
        hud.SetPlaybackState(playbackState);

        hud.Update(GetGameplayGameTime(gameTime));

        if (playbackState == GamePlaybackState.Paused)
            return;

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

        if (tutorialOverlay != null)
        {
            if (!tutorialWaveNotified && Ships.Count > 0)
            {
                tutorialOverlay.NotifyWaveStarted();
                tutorialWaveNotified = true;
            }
            tutorialOverlay.Update(gameTime);
        }

        // Only process tile clicks if the mouse is NOT over the upgrade menu
        if (!hud.IsMouseOverUpgradeMenu(input.MousePositionScaled))
        {
            buildManager.Update(
                input.MousePositionScaled,
                input.IsLeftClick(),
                input.IsCtrlHeld()
            );
        }
        else
        {
            // Still update the hovering logic so visuals don't freeze, but consume the click
            buildManager.Update(
                input.MousePositionScaled,
                false, // forcefully tell buildManager it's NOT a click because the UI consumed it
                input.IsCtrlHeld()
            );
        }
    }

    private GameTime GetGameplayGameTime(GameTime gameTime)
    {
        float scale = playbackState switch
        {
            GamePlaybackState.FastForward => 2f,
            GamePlaybackState.Paused => 0f,
            _ => 1f
        };

        long elapsedTicks = (long)(gameTime.ElapsedGameTime.Ticks * scale);
        return new GameTime(gameTime.TotalGameTime, TimeSpan.FromTicks(elapsedTicks));
    }

    private void CyclePlaybackState()
    {
        playbackState = playbackState switch
        {
            GamePlaybackState.Normal => GamePlaybackState.FastForward,
            GamePlaybackState.FastForward => GamePlaybackState.Paused,
            _ => GamePlaybackState.Normal
        };

        hud.SetPlaybackState(playbackState);
    }

    private bool HandlePause()
    {
        if (InputManager.Instance.IsActionJustPressed("Pause"))
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
                foreach (var troop in Ships[i].SpawnedTroops)
                    troop.GetWallAt = p => walls.TryGetValue(p, out var w) ? w : null;
                Troops.AddRange(Ships[i].SpawnedTroops);
                Ships[i].SpawnedTroops.Clear();
            }

            if (Ships[i].IsFinished)
            {
                Spawner.NotifyShipDeparted(Ships[i].AssignedCoast);
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
                Spawner.NotifyTroopDefeated();
            }

            if (troop.IsFinished && !troop.IsDead)
            {
                progress.TakeHits(1);
                Spawner.NotifyTroopDefeated();

                if (troop.CanBeRemoved)
                    Troops.RemoveAt(i);
            }

            if (troop.IsDead && troop.CanBeRemoved)
                Troops.RemoveAt(i);
        }
    }

    private void CheckLevelCompletion(GameTime gameTime)
    {
        progress.Update(gameTime, this);
        // Lose condition
        if (progress.IsLost())
        {
            AudioManager.Instance.StopBackgroundMusic();
            manager.SetScreen(new GameOverScreen(
                manager,
                this,
                currentLevel,
                ActiveLevelSet,
                false,
                Spawner.CurrentWave,
                progress.CoinsRemaining
            ));
            return;
        }


        // Win condition
        if (!levelCompleted && Spawner.IsAllWavesComplete && Ships.Count == 0 && Troops.Count == 0)
            progress.SetWon();

        if (!levelCompleted && progress.IsWon())
        {
            levelCompleted = true;

            AudioManager.Instance.StopBackgroundMusic();

            manager.SetScreen(new GameOverScreen(
                manager,
                this,
                currentLevel,
                ActiveLevelSet,
                true,
                Spawner.CurrentWave,
                progress.CoinsRemaining
            ));
            return;
        }
    }

    private void UpdateWalls()
    {
        foreach (var key in walls.Keys.ToList())
        {
            if (walls[key].IsDestroyed)
            {
                walls.Remove(key);
                occupiedTiles.Remove(key);
            }
        }
    }

    private void DrawWallHealthBars(SpriteBatch spriteBatch)
    {
        const int barHeight = 3;
        const int barYOffset = 4;

        foreach (var wall in walls.Values)
        {
            if (!wall.IsDamaged) continue;

            var center = grid.GridToWorld(wall.GridPos);
            int barWidth = grid.TileSize;
            int barX = (int)(center.X - barWidth / 2f);
            int barY = (int)(center.Y + grid.TileSize / 2f + barYOffset);

            float pct = (float)wall.Health / Wall.MaxHealth;
            int healthWidth = (int)(barWidth * pct);

            spriteBatch.Draw(Primitives.Pixel, new Rectangle(barX, barY, barWidth, barHeight), Color.Red);
            spriteBatch.Draw(Primitives.Pixel, new Rectangle(barX, barY, healthWidth, barHeight), Color.YellowGreen);
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

        spriteBatch.Draw(Primitives.Pixel,
            new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), (int)thickness),
            null,
            color,
            angle,
            new Vector2(0, 0.5f),
            SpriteEffects.None,
            0);
    }
}
