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
    private HashSet<Point> latestUntraverableHashSet = new();
    public static GameScreen Instance { get; private set; }
    public List<Explosion> Explosions = new();
    public List<NetEffect> NetEffects = new();
    public List<FireEffect> FireEffects = new();
    public List<FlameEffect> FlameEffects = new();
    public IEnumerable<Wall> Walls => walls.Values;

    private Dictionary<Point, BaseTower> placedTowers = new();

    private BaseTower selectedTower = null;
    private Wall selectedWall = null;

    private bool levelCompleted;

    private LevelProgressSystem progress;

    private Dictionary<Point, bool> occupiedTiles = new();

    private GamePlaybackState playbackState = GamePlaybackState.Normal;
    public List<Level> ActiveLevelSet { get; private set; }
    public Level CurrentLevel => currentLevel;

    private TutorialOverlay tutorialOverlay;
    private bool tutorialWaveNotified = false;

    private TowerUnlockManager towerUnlockManager;
    private InfoPopupOverlay infoPopup;
    private bool startTowerUnlockChecked = false;
    private TowerType? pendingHighlightTower;

    private InfoPopupOverlay troopEncounterPopup;
    private static readonly TroopType[] SpecialTroopTypes = [TroopType.Ghost, TroopType.Bomber, TroopType.Boss];
    private Troop highlightedTroop;
    private float spotlightPulseTimer;

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

        troopEncounterPopup = new InfoPopupOverlay();

        if (currentLevel.SaveKey?.StartsWith("grass_") == true)
        {
            infoPopup = new InfoPopupOverlay();
            infoPopup.OnContinue = () => hud.RefreshBuildMenu();
            towerUnlockManager = new TowerUnlockManager(currentLevel.Id, infoPopup);
        }

        hud = new Hud(buildManager, progress, Spawner,
            towerUnlockManager != null ? data => towerUnlockManager.IsAvailable(data.Type) : null);
        hud.SetPlaybackState(playbackState);
        hud.OnSpeedRequested = CyclePlaybackState;
        hud.OnMenuRequested = () => manager.SetScreen(new PauseScreen(manager, this, level: currentLevel, levelSet: ActiveLevelSet));

        if (towerUnlockManager != null)
            towerUnlockManager.OnTowerUnlocked += tower =>
            {
                pendingHighlightTower = tower;
            };

        wallRenderer = new WallRenderer(
            grid,
            currentLevel.Theme.Walls,
            walls
        );

        renderer.SetOccupiedTiles(occupiedTiles);

        buildManager.SetWallPlacementCallback(p =>
        {
            if (walls.ContainsKey(p) || placedTowers.ContainsKey(p))
            {
                buildManager.SetMode(BuildMode.None);
                SelectTile(p);
                return;
            }

            if (progress.CoinsRemaining >= BuildManager.WallCost)
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
                refundAmount = (int)Math.Ceiling(BuildManager.WallCost * Primitives.RefundBuildingPrc);
                walls.Remove(p);
            }
            else if (placedTowers.TryGetValue(p, out BaseTower tower))
            {
                refundAmount = (int)Math.Ceiling(tower.Data.Cost * Primitives.RefundBuildingPrc);
                placedTowers.Remove(p);
            }

            if (refundAmount > 0)
            {
                occupiedTiles.Remove(p);
                progress.AddCoins(refundAmount);
                AudioManager.Instance.PlayRandomImpact();
            }
        });

        buildManager.SetTowerPlacementCallback((p, data) =>
        {
            bool isFreeTower = towerUnlockManager?.PendingFreeTower == data.Type;

            if (placedTowers.ContainsKey(p) || walls.ContainsKey(p))
            {
                buildManager.SetMode(BuildMode.None);
                SelectTile(p);
                return;
            }

            if (isFreeTower || progress.CoinsRemaining >= data.Cost)
            {
                currentLevel.Decorations.RemoveAll(d => d.GridPos == p);
                placedTowers[p] = TowerFactory.Create(data, grid.GridToWorld(p), Troops);
                occupiedTiles[p] = true;

                if (isFreeTower)
                {
                    towerUnlockManager.ClearFreeTower();
                    SaveManager.MarkFreeTowerPlacementClaimed(data.Type);
                }
                else
                {
                    progress.SpendCoins(data.Cost);
                }

                AudioManager.Instance.PlayRandomImpact();

                selectedTower = null;
                selectedWall = null;

                if (!buildManager.CtrlHeld)
                {
                    buildManager.SetMode(BuildMode.None);
                }
            }
        });

        buildManager.SetSelectCallback(SelectTile);

        if (currentLevel.Id == 1)
            tutorialOverlay = new TutorialOverlay();

        AudioManager.Instance.PlayBackgroundMusic("battle_music");
    }

    public override void Update(GameTime gameTime)
    {
        if (HandlePause()) return;

        HandlePlaybackShortcuts();

        UpdateBuildSystem(gameTime);

        if (playbackState == GamePlaybackState.Paused)
            return;

        var gameplayGameTime = GetGameplayGameTime(gameTime);

        UpdateWalls();
        UpdateTowers(gameplayGameTime);
        if (!ShouldFreezeGameplayForTutorial() && !ShouldFreezeGameplayForPopup())
        {
            UpdateSpawner(gameplayGameTime);
            UpdateShips(gameplayGameTime);
            UpdateTroops(gameplayGameTime);
        }

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

        for (int i = FlameEffects.Count - 1; i >= 0; i--)
        {
            FlameEffects[i].Update(gameTime);
            if (FlameEffects[i].IsFinished)
                FlameEffects.RemoveAt(i);
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

        foreach (var flame in FlameEffects)
            flame.Draw(spriteBatch);

        DrawWallHealthBars(spriteBatch);

        var overlayRenderer = renderer.GetOverlayRenderer();
        if (overlayRenderer != null)
        {
            overlayRenderer.Draw(spriteBatch);
        }

        if (troopEncounterPopup?.IsActive == true && highlightedTroop != null && !highlightedTroop.IsDead)
            DrawTroopSpotlight(spriteBatch);

        hud.Draw(spriteBatch);
        tutorialOverlay?.Draw(spriteBatch);
        infoPopup?.Draw(spriteBatch);
        troopEncounterPopup?.Draw(spriteBatch);
    }

    private void SelectTile(Point p)
    {
        if (placedTowers.TryGetValue(p, out BaseTower tower))
        {
            selectedTower = tower;
            selectedWall = null;
        }
        else if (walls.TryGetValue(p, out Wall wall))
        {
            selectedWall = wall;
            selectedTower = null;
        }
        else
        {
            selectedTower = null;
            selectedWall = null;
        }
        hud.SetSelectedWall(selectedWall);
        hud.SetSelectedTower(selectedTower);
    }

    private void UpdateBuildSystem(GameTime gameTime)
    {
        wallRenderer.Update(gameTime);

        // If mode is reset to something else (e.g. wall building), clear selection
        if (buildManager.GetMode() != BuildMode.None)
        {
            selectedTower = null;
            selectedWall = null;
            hud.SetSelectedWall(null);
        }

        hud.SetSelectedTower(selectedTower);
        hud.SetPlaybackState(playbackState);
        hud.FreeTowerType = towerUnlockManager?.PendingFreeTower;
        hud.HighlightedTower = infoPopup?.IsActive == true ? pendingHighlightTower : null;

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

        // Handle wall repair interaction
        if (selectedWall != null && hud.WasRepairClicked())
        {
            int cost = selectedWall.GetRepairCostToFull();
            if (cost > 0 && progress.CoinsRemaining >= cost && !selectedWall.IsDestroyed)
            {
                progress.SpendCoins(cost);
                selectedWall.RepairToFull();
                AudioManager.Instance.PlayRandomImpact();
            }
        }

        // Handle wall upgrade interaction
        if (selectedWall != null && hud.WasWallUpgradeClicked())
        {
            int cost = selectedWall.GetUpgradeCost();
            if (cost > 0 && progress.CoinsRemaining >= cost && selectedWall.CanUpgrade)
            {
                progress.SpendCoins(cost);
                selectedWall.ApplyUpgrade();
                AudioManager.Instance.PlayRandomImpact();
            }
        }

        if (selectedTower != null && hud.WasTargetModeClicked())
        {
            selectedTower.CycleAttackMode();
            AudioManager.Instance.PlayRandomImpact();
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

        if (towerUnlockManager != null)
        {
            if (!startTowerUnlockChecked && tutorialOverlay?.IsIntroActive != true)
            {
                towerUnlockManager.CheckStartUnlock();
                startTowerUnlockChecked = true;
            }

            towerUnlockManager.CheckWaveUnlock(Spawner.CurrentWave);
        }

        infoPopup?.Update(gameTime);
        troopEncounterPopup?.Update(gameTime);

        if (troopEncounterPopup?.IsActive == true)
            spotlightPulseTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Only process tile clicks if the mouse is NOT over the upgrade menu
        if (!hud.IsMouseOverUpgradeMenu(input.MousePositionScaled))
        {
            buildManager.Update(
                input.MousePositionScaled,
                input.IsLeftClick(),
                input.IsCtrlHeld(),
                input.IsRightClick()
            );
        }
        else
        {
            // Still update the hovering logic so visuals don't freeze, but consume the click
            buildManager.Update(
                input.MousePositionScaled,
                false, // forcefully tell buildManager it's NOT a click because the UI consumed it
                input.IsCtrlHeld(),
                input.IsRightClick()
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
            GamePlaybackState.FastForward => GamePlaybackState.Normal,
            _ => GamePlaybackState.Normal
        };

        hud.SetPlaybackState(playbackState);
    }

    private void HandlePlaybackShortcuts()
    {
        if (InputManager.Instance.IsActionJustPressed("TogglePause"))
        {
            playbackState = playbackState == GamePlaybackState.Paused
                ? GamePlaybackState.Normal
                : GamePlaybackState.Paused;

            hud.SetPlaybackState(playbackState);
        }

        if (InputManager.Instance.IsActionJustPressed("ToggleFastForward") && playbackState != GamePlaybackState.Paused)
        {
            CyclePlaybackState();
        }
    }

    private bool HandlePause()
    {
        if (InputManager.Instance.IsActionJustPressed("Pause"))
        {
            manager.SetScreen(new PauseScreen(manager, this, level: currentLevel, levelSet: ActiveLevelSet));
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
                {
                    troop.GetWallAt = p => walls.TryGetValue(p, out var w) ? w : null;
                    troop.Died += OnTroopDied;
                    CheckTroopEncounter(troop);
                }
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

            if (troop.IsDead)
            {
                if (!troop.HasDroppedReward)
                {
                    hud.GetCoinManager().SpawnCoin(troop.Position, troop.CoinValue);
                    troop.MarkRewardGiven();
                    Spawner.NotifyTroopDefeated();
                }
                if (troop.CanBeRemoved)
                    Troops.RemoveAt(i);
                continue;
            }

            if (troop.IsFinished)
            {
                // progress.TakeHits(1);
                // Spawner.NotifyTroopDefeated();
                Troops.RemoveAt(i);
            }
        }
    }

    private void CheckLevelCompletion(GameTime gameTime)
    {
        progress.Update(gameTime, this);
        // Lose condition
        if (progress.IsLost())
        {
            AudioManager.Instance.PlayBackgroundMusic("lose_music");
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

            AudioManager.Instance.PlayBackgroundMusic("win_music");

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

        foreach (var wall in walls.Values)
        {
            if (!wall.IsDamaged) continue;

            var center = grid.GridToWorld(wall.GridPos);
            int barWidth = grid.TileSize;
            int barX = (int)(center.X - barWidth / 2f);
            int barY = (int)(center.Y - barHeight / 2f);

            float pct = (float)wall.Health / wall.MaxHealth;
            int healthWidth = (int)(barWidth * pct);

            spriteBatch.Draw(Primitives.Pixel, new Rectangle(barX, barY, barWidth, barHeight), Color.Red);
            spriteBatch.Draw(Primitives.Pixel, new Rectangle(barX, barY, healthWidth, barHeight), Color.YellowGreen);
        }
    }

    private void OnTroopDied(Troop troop)
    {
        foreach (var tower in placedTowers.Values)
            tower.NotifyTroopDied(troop);
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

    private void DrawCircle(SpriteBatch spriteBatch, Vector2 center, float radius, Color color, float thickness = 2f)
    {
        int points = 32;
        float step = MathHelper.TwoPi / points;

        for (int i = 0; i < points; i++)
        {
            float angle1 = i * step;
            float angle2 = (i + 1) * step;

            Vector2 p1 = new Vector2((float)Math.Cos(angle1), (float)Math.Sin(angle1)) * radius;
            Vector2 p2 = new Vector2((float)Math.Cos(angle2), (float)Math.Sin(angle2)) * radius;

            DrawLine(spriteBatch, center + p1, center + p2, color, thickness);
        }
    }

    private void DrawTroopSpotlight(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Primitives.Pixel, new Rectangle(0, 0, RumGame.VirtualWidth, RumGame.VirtualHeight), Color.Black * 0.65f);

        highlightedTroop.Draw(spriteBatch);

        float pulse = 0.5f + 0.5f * (float)Math.Sin(spotlightPulseTimer * 4f);
        float radius = highlightedTroop.Size.X * (0.7f + pulse * 0.15f);
        var ringColor = Color.Lerp(Color.OrangeRed, Color.Yellow, pulse);

        DrawCircle(spriteBatch, highlightedTroop.Position, radius, ringColor, 4f);
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

    private bool ShouldFreezeGameplayForTutorial()
    {
        return tutorialOverlay?.IsIntroActive == true;
    }

    private bool ShouldFreezeGameplayForPopup()
    {
        return infoPopup?.IsActive == true || troopEncounterPopup?.IsActive == true;
    }

    private void CheckTroopEncounter(Troop troop)
    {
        var data = troop.Data;
        if (!SpecialTroopTypes.Contains(data.Type) || SaveManager.IsTroopEncountered(data.Type))
            return;

        SaveManager.MarkTroopEncountered(data.Type);
        highlightedTroop = troop;
        troopEncounterPopup.Show($"New enemy: {data.Type}", data.Description);
    }
}
