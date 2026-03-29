using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace RumDefence;

public class GameScreen : Screen
{
    private Grid grid;
    private GridRenderer renderer;
    private Level currentLevel;

    public ShipSpawner Spawner { get; private set;  }

    public List<Ship> Ships { get; private set; } = new();
    public List<Troop> Troops { get; private set; }  = new(); 

    private bool levelCompleted;
    
    private LevelProgressSystem progress;

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

        renderer = new GridRenderer(currentLevel.Theme);

        Spawner = new ShipSpawner(currentLevel, grid);
        progress = new (currentLevel.StartingLives, currentLevel.StartingCoinBalance);
    }

    public override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Escape))
        {
            manager.SetScreen(new PauseScreen(manager, this));
            return;
        }

        var newShip = Spawner.Update(gameTime);
        if (newShip != null)
        {
            Ships.Add(newShip);
        }

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

        for (int i = Troops.Count - 1; i >= 0; i--)
        {
            var troop = Troops[i]; 
            troop.Update(gameTime);

            if (troop.IsFinished || troop.IsDead)
            {
                // TODO: Base the hits on the damage stat of the troop
                if (troop.IsFinished) progress.TakeHits(1);
                
                Troops.RemoveAt(i);
            }
        }
        
        progress.Update(gameTime, this);

        // TODO: Do not ignore IsLost after testing
        levelCompleted = progress.IsWon() /*|| progress.IsLost()*/;

        if (progress.IsWon())
        {
            UnlockNextLevel();
        }

        if (levelCompleted)
        {
            // TODO: Show win or lose screen based
            manager.SetScreen(new MainMenuScreen(manager));
        }
        
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(new Color(30, 144, 255));

        renderer.Draw(grid, spriteBatch);

        foreach (var ship in Ships)
            ship.Draw(spriteBatch);

        foreach (var troop in Troops) 
            troop.Draw(spriteBatch);
    }

    private void UnlockNextLevel()
    {
        int currentIndex = GrassLevels.All.IndexOf(currentLevel);

        if (currentIndex + 1 < GrassLevels.All.Count)
        {
            GrassLevels.All[currentIndex + 1].IsUnlocked = true;
        }
    }
}