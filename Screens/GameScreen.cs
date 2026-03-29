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

    private ShipSpawner spawner;

    private List<Ship> ships = new();
    private List<Troop> troops = new(); 

    private bool levelCompleted = false;
    
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

        spawner = new ShipSpawner(currentLevel, grid);
        progress = new (10);
    }

    public override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Escape))
        {
            manager.SetScreen(new PauseScreen(manager, this));
            return;
        }

        var newShip = spawner.Update(gameTime);
        if (newShip != null)
        {
            ships.Add(newShip);
        }

        for (int i = ships.Count - 1; i >= 0; i--)
        {
            ships[i].Update(gameTime);

            if (ships[i].SpawnedTroops.Count > 0)
            {
                troops.AddRange(ships[i].SpawnedTroops);
                ships[i].SpawnedTroops.Clear();
            }

            if (ships[i].IsFinished)
            {
                ships.RemoveAt(i);
            }
        }

        for (int i = troops.Count - 1; i >= 0; i--)
        {
            var troop = troops[i]; 
            troop.Update(gameTime);

            if (troop.IsFinished || troop.IsDead)
            {
                // TODO: Base the hits on the damage stat of the troop
                if (troop.IsFinished) progress.TakeHits(1);
                
                troops.RemoveAt(i);
            }
        }

        levelCompleted = progress.IsWon() || progress.IsLost();

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

        foreach (var ship in ships)
            ship.Draw(spriteBatch);

        foreach (var troop in troops) 
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