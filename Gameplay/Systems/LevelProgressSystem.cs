using System;

namespace RumDefence;

public class LevelProgressSystem
{
    
    /// <summary>
    /// Indicates the number of lives the player has remaining.
    /// </summary>
    private int LivesRemaining { get; set; }
    
    /// <summary>
    /// Indicates the total number of lives the player had at the start of the level.
    /// </summary>
    private int LivesTotal { get; set; }
    
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
        return false;
    }

    /// <summary>
    /// Return whether the level is lost or not. A level is considered lost if the player loses all their lives.
    /// </summary>
    /// <returns>A boolean indicating if the level is lost or not</returns>
    public bool IsLost()
    {
        return LivesRemaining <= 0;
    }
    
}