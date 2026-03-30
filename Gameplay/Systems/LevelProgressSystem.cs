using System;
using System.Linq;
using Microsoft.Xna.Framework;
using RumDefence.Exceptions;

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
    /// Indicates the number of coins the player has collected so far in the level.
    /// </summary>
    public int CoinsRemaining { get; private set; }

    /// <summary>
    /// Indicates whether the level has been won or not.
    /// </summary>
    private bool levelWon;

    /// <param name="initialLives">The initial amount of lives at the start of the level</param>
    /// <param name="initialCoins">The initial amount of coins at the start of the level</param>
    public LevelProgressSystem(int initialLives, int initialCoins)
    {
        LivesRemaining = initialLives;
        LivesTotal = initialLives;
        CoinsRemaining = initialCoins;
    }

    /// <summary>
    /// Make the player take a certain amount of hits, reducing their remaining lives accordingly.
    /// When the player takes more hits than they have remaining, their lives will be reduced to 0, but not below.
    /// </summary>
    /// <param name="hits">Amount of hits to take, hits must be a positive integer</param>
    /// <exception cref="ArgumentException">Thrown when the amount of hits is negative</exception>
    public void TakeHits(int hits)
    {
        if (hits < 0)
        {
            throw new ArgumentException("Hits cannot be negative");
        }

        LivesRemaining = Math.Max(LivesRemaining - hits, 0);
    }

    /// <summary>
    /// Add a certain amount of coins to the player's total, increasing their remaining coins accordingly.
    /// </summary>
    /// <param name="coins">Amount of coins to add, coins must be a positive integer</param>
    /// <exception cref="ArgumentException">Thrown when the amount of coins is negative</exception>
    public void AddCoins(int coins)
    {
        if (coins < 0)
        {
            throw new ArgumentException("Coins cannot be negative");
        }

        CoinsRemaining += coins;
    }

    /// <summary>
    /// Deduct a certain amount of coins from the player's total, reducing their remaining coins accordingly.
    /// </summary>
    /// <param name="coins">Amount of coins to remove, must be a positive integer</param>
    /// <exception cref="ArgumentException">Thrown when the amount of coins is negative</exception>
    /// <exception cref="ArgumentException">Thrown when the amount of coins to spend is greater than the remaining coins</exception>
    public void SpendCoins(int coins)
    {
        if (coins < 0)
        {
            throw new ArgumentException("Coins cannot be negative");
        }

        if (coins > CoinsRemaining)
        {
            throw new InsufficientBalanceException("Not enough coins remaining to spend");
        }

        CoinsRemaining -= coins;
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