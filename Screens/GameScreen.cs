using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rum_Defence.Entities.Towers;
using System;
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

    //remove when hud is done
    private List<CannonTower> testTowers;

    private bool levelCompleted = false;

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

        //remove when hud is done now only spawn at level 3
        testTowers = currentLevel.Id == 3 ? new List<CannonTower>()
        {
            new (new Vector2(1500, 300), troops),
            new (new Vector2(1500, 900), troops),
            new (new Vector2(1700, 500), troops),

        } : new();

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
            troops[i].Update(gameTime);

            if (troops[i].IsFinished || troops[i].IsDead)
            {
                troops.RemoveAt(i);
            }
        }

        if (!levelCompleted && spawner.IsFinished && ships.Count == 0 && troops.Count == 0)
        {
            levelCompleted = true;
            UnlockNextLevel();
        }

        testTowers.ForEach(x => x.Update(gameTime));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(new Color(30, 144, 255));

        renderer.Draw(grid, spriteBatch);

        foreach (var ship in ships)
            ship.Draw(spriteBatch);

        foreach (var troop in troops)
            troop.Draw(spriteBatch);


        testTowers.ForEach(x => x.Draw(spriteBatch));
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