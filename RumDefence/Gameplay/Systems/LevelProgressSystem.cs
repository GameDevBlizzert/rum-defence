using Microsoft.Xna.Framework;
using RumDefence.Exceptions;
using RumDefence.Gameplay.Levels.Dev;
using RumDefence.Gameplay.Levels.Ghost;
using RumDefence.Levels.Ghost;
using RumDefence.Levels.Grass;
using RumDefence.Levels.Infinity;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class LevelProgressSystem : IGameLoopSystem
{
    /// <summary>
    /// Indicates the number of lives the player has remaining.
    /// </summary>
    public float LivesRemaining { get; private set; }

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

    /// <summary>
    /// Ensures the next level is only unlocked once per win.
    /// </summary>
    private bool hasUnlockedNext = false;

    private List<Level> activeLevelSet;
    private Level currentLevel;
    /// <summary>
    /// Contains all level sets (themes) for progression handling.
    /// </summary>
    private static List<List<Level>> allLevelSets = new()
    {
        DevLevels.All,
        GrassLevels.All,
        GhostLevels.All,
        InfinityLevels.All
    };

    /// <param name="initialLives">The initial amount of lives at the start of the level</param>
    /// <param name="initialCoins">The initial amount of coins at the start of the level</param>
    public LevelProgressSystem(int initialLives, int initialCoins, List<Level> levelSet, Level level)
    {
        LivesRemaining = initialLives;
        LivesTotal = initialLives;
        CoinsRemaining = initialCoins;

        activeLevelSet = levelSet;
        currentLevel = level;
    }

    /// <summary>
    /// Make the player take a certain amount of hits, reducing their remaining lives accordingly.
    /// When the player takes more hits than they have remaining, their lives will be reduced to 0, but not below.
    /// </summary>
    /// <param name="hits">Amount of hits to take, hits must be a positive integer</param>
    /// <exception cref="ArgumentException">Thrown when the amount of hits is negative</exception>
    public void TakeHits(float hits)
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
    /// Return whether the level is won or not.
    /// A level is considered won if:
    /// - All waves have finished spawning
    /// - There are no ships left
    /// - There are no troops left
    /// </summary>
    /// <returns>A boolean indicating if the level is won or not</returns>
    public bool IsWon()
    {
        if (IsLost()) return false;

        return levelWon;
    }

    /// <summary>
    /// Return whether the level is lost or not.
    /// A level is considered lost if the player loses all their lives.
    /// </summary>
    /// <returns>A boolean indicating if the level is lost or not</returns>
    public bool IsLost()
    {
        return LivesRemaining <= 0;
    }

    /// <summary>
    /// Manually set the level as won.
    /// </summary>
    public void SetWon()
    {
        levelWon = true;
    }

    /// <summary>
    /// Updates the level progress system.
    /// Checks if the win condition has been met.
    /// Also handles unlocking the next level in the current theme.
    /// </summary>
    public void Update(GameTime gameTime, GameScreen gameScreen)
    {
        // - Spawner finished (no more enemies spawning)
        // - No ships left in the game
        // - No troops left in the game
        if (gameScreen.Spawner.IsAllWavesComplete &&
            gameScreen.Ships.Count == 0 &&
            gameScreen.Troops.Count == 0)
        {
            if (!levelWon)
            {
                levelWon = true;

                // Ensure unlock happens only once
                if (!hasUnlockedNext)
                {
                    UnlockNextLevel();
                    hasUnlockedNext = true;
                }
            }
        }
    }

    /// <summary>
    /// Unlocks the next level within the same theme.
    /// Will only unlock if this is the highest unlocked level (prevents replay abuse).
    /// </summary>
    private void UnlockNextLevel()
    {
        int index = activeLevelSet.FindIndex(l => l.Id == currentLevel.Id);
        if (index == -1) return;

        for (int i = index + 1; i < activeLevelSet.Count; i++)
        {
            if (activeLevelSet[i].IsUnlocked)
                return;
        }

        if (index + 1 < activeLevelSet.Count)
        {
            var next = activeLevelSet[index + 1];
            next.IsUnlocked = true;

            if (next.SaveKey != null)
            {
                SaveManager.CurrentSave.UnlockedLevelKeys.Add(next.SaveKey);
                SaveManager.Save();
            }
        }
    }
}