using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace RumDefence;

public class LevelProgressSystem : IGameLoopSystem
{
    /// <summary>
    /// Indicates the number of lives the player has remaining.
    /// </summary>
    public int LivesRemaining { get; private set; }

    /// <summary>
    /// Indicates the total number of lives the player had at the start of the level.
    /// </summary>
    public int LivesTotal { get; private set; }

    /// <summary>
    /// Indicates whether the level has been won or not.
    /// </summary>
    private bool levelWon;

    /// <param name="initialLives">The initial amount of lives at the start of the level</param>
    public LevelProgressSystem(int initialLives)
    {
        LivesRemaining = initialLives;
        LivesTotal = initialLives;
    }

    /// <summary>
    /// Let the player take a certain amount of hits, reducing their remaining lives accordingly.
    /// </summary>
    /// <param name="hits">Amount of hits to take, hits must be a positive integer</param>
    /// <exception cref="ArgumentException">Thrown when the amount of hits is negative</exception>
    public void TakeHits(int hits)
    {
        if (hits < 0)
        {
            throw new ArgumentException("Hits cannot be negative");
        }

        LivesRemaining -= hits;
    }

    /// <summary>
    /// Return wheter the level is won or not.
    /// A level is considered won if all ship waves and bosses have been defeated.
    /// </summary>
    /// <returns>A boolean indicating if the level is won or not</returns>
    public bool IsWon()
    {
        // TODO: Add other relevant checks
        return levelWon;
    }

    /// <summary>
    /// Return whether the level is lost or not. A level is considered lost if the player loses all their lives.
    /// </summary>
    /// <returns>A boolean indicating if the level is lost or not</returns>
    public bool IsLost()
    {
        return LivesRemaining <= 0;
    }

    public void Update(GameTime gameTime, GameScreen gameScreen)
    {
        if (gameScreen.Spawner.IsFinished &&
            gameScreen.Ships.Count(s => 
                s.State == Ship.ShipState.SailingToDock || s.State == Ship.ShipState.SailingToDock || s.State == Ship.ShipState.Unloading) == 0 &&
            gameScreen.Troops.Count == 0)
            levelWon = true;
    }
}