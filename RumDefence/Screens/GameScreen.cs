using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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

    private List<BaseTower> towers = new();
    private HashSet<Point> occupiedTowerTiles = new();

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

        Spawner = new ShipSpawner(currentLevel, grid);

        hud = new Hud(buildManager);

        wallRenderer = new WallRenderer(
            grid,
            currentLevel.Theme.Walls,
            walls
        );

        buildManager.SetWallPlacementCallback(p =>
        {
            if (!walls.ContainsKey(p))
            {
                walls[p] = new Wall(p);
                // Play random impact sound when wall is placed
                AudioManager.Instance.PlayRandomImpact();
            }
        });

        buildManager.SetMusketTowerPlacementCallback(p =>
        {
            if (occupiedTowerTiles.Add(p))
                towers.Add(new MusketTower(grid.GridToWorld(p), Troops));
        });

        buildManager.SetCannonTowerPlacementCallback(p =>
        {
            if (occupiedTowerTiles.Add(p))
                towers.Add(new CannonTower(grid.GridToWorld(p), Troops));
        });

        Spawner = new ShipSpawner(currentLevel, grid);
        progress = new(currentLevel.StartingLives, currentLevel.StartingCoinBalance);

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
        CheckLevelCompletion(gameTime);
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

        hud.Draw(spriteBatch);
        foreach (var tower in towers)
            tower.Draw(spriteBatch);
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
        var newShip = Spawner.Update(gameTime);

        if (newShip != null)
        {
            Ships.Add(newShip);
        }
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
        var updatePaths = !latestUntraverableHashSet.Equals(untraversable);

        latestUntraverableHashSet = untraversable;

        for (int i = Troops.Count - 1; i >= 0; i--)
        {
            var troop = Troops[i];
            troop.Update(gameTime);

            if (updatePaths)
                troop.UpdatePathfinding(latestUntraverableHashSet);

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

    private void CheckLevelCompletion(GameTime gameTime)
    {
        if (!levelCompleted && Spawner.IsFinished && Ships.Count == 0 && Troops.Count == 0)
            progress.Update(gameTime, this);

        // TODO: Do not ignore IsLost after testing
        levelCompleted = progress.IsWon() /*|| progress.IsLost()*/;

        if (progress.IsWon())
        {
            UnlockNextLevel();
        }

        foreach (var tower in towers)
            tower.Update(gameTime);

        if (levelCompleted)
        {
            // Stop background music when leaving game screen
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

        foreach (var tower in towers)
        {
            var tile = grid.WorldToGrid(tower.Position);
            if (tile != null)
                untraversable.Add(tile.Value);
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

}
