using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Moq;
using RumDefence;
using RumDefence.Exceptions;
using Xunit;

namespace RumDefence.Tests.Gameplay.Systems;

[TestSubject(typeof(LevelProgressSystem))]
public class LevelProgressSystemTest
{

    [Theory]
    [InlineData(10, 3, 7)]
    [InlineData(5, 5, 0)]
    [InlineData(10, 8, 2)]
    [InlineData(1, 2, 0)]
    [InlineData(1, 0, 1)]
    public void TakeHits_WithPositiveHits_HitsShouldBeTaken(int startingHealth, int hits, int expectedHealth)
    {
        var levelProgressSystem = new LevelProgressSystem(startingHealth, 0);
        levelProgressSystem.TakeHits(hits);
        Assert.Equal(expectedHealth, levelProgressSystem.LivesRemaining);
    }
    
    [Theory]
    [InlineData(10, -3)]
    [InlineData(5, -5)]
    public void TakeHits_WithNegativeHits_ShouldThrowArgumentException(int startingHealth, int hits)
    {
        var levelProgressSystem = new LevelProgressSystem(startingHealth, 0);
        Assert.Throws<ArgumentException>(() => levelProgressSystem.TakeHits(hits));
    }

    [Theory]
    [InlineData(10, 3, 13)]
    [InlineData(5, 5, 10)]
    [InlineData(10, 8, 18)]
    [InlineData(1, 0, 1)]
    public void AddCoins_WithPositiveValues_CoinsShouldBeAdded(int startingCoins, int coinsToAdd, int expectedCoins)
    {
        var levelProgressSystem = new LevelProgressSystem(0, startingCoins);
        levelProgressSystem.AddCoins(coinsToAdd);
        Assert.Equal(expectedCoins, levelProgressSystem.CoinsRemaining); 
    }

    [Theory]
    [InlineData(10, -3)]
    [InlineData(5, -5)]
    public void AddCoins_WithNegativeValues_ShouldThrowArgumentException(int startingCoins, int coinsToAdd)
    {
        var levelProgressSystem = new LevelProgressSystem(0, startingCoins);
        Assert.Throws<ArgumentException>(() => levelProgressSystem.AddCoins(coinsToAdd));
    }
    
    [Fact]
    public void Constructor_ShouldInitializeLivesAndCoins()
    {
        var levelProgressSystem = new LevelProgressSystem(12, 34);
    
        Assert.Equal(12, levelProgressSystem.LivesRemaining);
        Assert.Equal(34, levelProgressSystem.CoinsRemaining);
    }
    
    [Theory]
    [InlineData(10, 0, 10)]
    [InlineData(0, 5, 0)]
    [InlineData(1, 1, 0)]
    public void TakeHits_WithBoundaryValues_ShouldClampAtZero(int startingHealth, int hits, int expectedHealth)
    {
        var levelProgressSystem = new LevelProgressSystem(startingHealth, 0);
    
        levelProgressSystem.TakeHits(hits);
    
        Assert.Equal(expectedHealth, levelProgressSystem.LivesRemaining);
    }
    
    [Fact]
    public void TakeHits_CalledMultipleTimes_ShouldAccumulateDamageAndClampAtZero()
    {
        var levelProgressSystem = new LevelProgressSystem(10, 0);
    
        levelProgressSystem.TakeHits(3);
        levelProgressSystem.TakeHits(4);
        levelProgressSystem.TakeHits(10);
    
        Assert.Equal(0, levelProgressSystem.LivesRemaining);
    }
    
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(100, 0, 100)]
    public void AddCoins_WithZeroValue_ShouldKeepCoinsUnchanged(int startingCoins, int coinsToAdd, int expectedCoins)
    {
        var levelProgressSystem = new LevelProgressSystem(0, startingCoins);
    
        levelProgressSystem.AddCoins(coinsToAdd);
    
        Assert.Equal(expectedCoins, levelProgressSystem.CoinsRemaining);
    }
    
    [Fact]
    public void AddCoins_CalledMultipleTimes_ShouldAccumulateCoins()
    {
        var levelProgressSystem = new LevelProgressSystem(0, 5);
    
        levelProgressSystem.AddCoins(10);
        levelProgressSystem.AddCoins(15);
        levelProgressSystem.AddCoins(0);
    
        Assert.Equal(30, levelProgressSystem.CoinsRemaining);
    }
    
    [Fact]
    public void AddCoins_AndTakeHits_ShouldUpdateIndependentStateValues()
    {
        var levelProgressSystem = new LevelProgressSystem(10, 20);
    
        levelProgressSystem.TakeHits(4);
        levelProgressSystem.AddCoins(7);
        levelProgressSystem.TakeHits(3);
        levelProgressSystem.AddCoins(8);
    
        Assert.Equal(3, levelProgressSystem.LivesRemaining);
        Assert.Equal(35, levelProgressSystem.CoinsRemaining);
    }
    
    [Theory]
    [InlineData(10, 3, 7)]
    [InlineData(5, 5, 0)]
    [InlineData(10, 1, 9)]
    public void SpendCoins_WithPositiveValues_CoinsShouldIncreaseByValue(int startingCoins, int coinsToSpend, int expectedCoins)
    {
        var levelProgressSystem = new LevelProgressSystem(10, startingCoins);
        levelProgressSystem.SpendCoins(coinsToSpend);
        Assert.Equal(expectedCoins, levelProgressSystem.CoinsRemaining);
    }
    
    [Theory]
    [InlineData(10, -10)]
    [InlineData(5, -3)]
    public void SpendCoins_WithZeroOrNegativeValues_ShouldThrowArgumentException(int startingCoins, int coinsToSpend)
    {
        var levelProgressSystem = new LevelProgressSystem(10, startingCoins);
        Assert.Throws<ArgumentException>(() => levelProgressSystem.SpendCoins(coinsToSpend));
    }
    
    [Theory]
    [InlineData(0, 1)]
    [InlineData(5, 6)]
    [InlineData(10, 100)]
    public void SpendCoins_WithValuesGreaterThanRemainingCoins_ShouldThrowArgumentException(int startingCoins, int coinsToSpend)
    {
        var levelProgressSystem = new LevelProgressSystem(10, startingCoins);
        Assert.Throws<InsufficientBalanceException>(() => levelProgressSystem.SpendCoins(coinsToSpend));
    }
    
    [Fact]
    public void IsWon_BeforeUpdate_ShouldReturnFalse()
    {
        var levelProgressSystem = new LevelProgressSystem(10, 0);
        Assert.False(levelProgressSystem.IsWon());
    }
    
    [Theory]
    [InlineData(1, false)]
    [InlineData(0, true)]
    [InlineData(-1, true)]
    [InlineData(5, false)]
    public void IsLost_ShouldReturnTrueWhenLivesAreZeroOrLess(int startingLives, bool expectedLost)
    {
        var levelProgressSystem = new LevelProgressSystem(startingLives, 0);
        Assert.Equal(expectedLost, levelProgressSystem.IsLost());
    }
    
    [Fact]
    public void IsLost_AfterTakingHitsToZero_ShouldReturnTrue()
    {
        var levelProgressSystem = new LevelProgressSystem(3, 0);
        levelProgressSystem.TakeHits(3);
        Assert.True(levelProgressSystem.IsLost());
    }
    
    [Fact]
    public void IsLost_AfterPartialHits_ShouldReturnFalse()
    {
        var levelProgressSystem = new LevelProgressSystem(3, 0);
        levelProgressSystem.TakeHits(2);
        Assert.False(levelProgressSystem.IsLost());
    }
    
}