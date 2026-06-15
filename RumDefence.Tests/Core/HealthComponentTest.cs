// AI generated: this test file was created with AI assistance.
using System;
using JetBrains.Annotations;
using Xunit;

namespace RumDefence.Tests.Core;

[TestSubject(typeof(HealthComponent))]
public class HealthComponentTest
{
    [Fact]
    public void Constructor_ShouldInitializeCurrentToInitialHealth()
    {
        var health = new HealthComponent(80);

        Assert.Equal(80, health.Current);
        Assert.Equal(80, health.InitialHealth);
        Assert.False(health.IsDead);
    }

    [Theory]
    [InlineData(100, 30, 70)]
    [InlineData(100, 100, 0)]
    [InlineData(50, 75, -25)]
    public void TakeDamage_WithPositiveAmount_ShouldReduceCurrent(int initial, float damage, float expected)
    {
        var health = new HealthComponent(initial);

        health.TakeDamage(damage);

        Assert.Equal(expected, health.Current);
    }

    [Fact]
    public void TakeDamage_WithNegativeAmount_ShouldThrowArgumentException()
    {
        var health = new HealthComponent(100);

        Assert.Throws<ArgumentException>(() => health.TakeDamage(-1));
    }

    [Theory]
    [InlineData(100, 99, false)]
    [InlineData(100, 100, true)]
    [InlineData(100, 101, true)]
    public void IsDead_ShouldReflectWhetherCurrentReachedZero(int initial, float damage, bool expectedDead)
    {
        var health = new HealthComponent(initial);

        health.TakeDamage(damage);

        Assert.Equal(expectedDead, health.IsDead);
    }

    [Fact]
    public void Heal_WithPositiveAmount_ShouldIncreaseCurrent()
    {
        var health = new HealthComponent(100);
        health.TakeDamage(60);

        health.Heal(30);

        Assert.Equal(70, health.Current);
    }

    [Fact]
    public void Heal_WithNegativeAmount_ShouldThrowArgumentException()
    {
        var health = new HealthComponent(100);

        Assert.Throws<ArgumentException>(() => health.Heal(-1));
    }
}
