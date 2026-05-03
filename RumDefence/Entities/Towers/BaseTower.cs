using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class BaseTower : Entity
{
    protected readonly List<Troop> Troops;
    protected readonly List<Projectile> Projectiles = [];

    public int CurrentLevel { get; protected set; } = 0;
    public int MaxLevel { get; protected set; } = 3;

    protected float BaseRange = 700f;
    protected float RangeUpgradeFlat = 50f;
    protected float RangeUpgradePercent = 0.1f;

    protected float BaseFireRate = 1f; // shots per second
    protected float FireRateUpgradeFlat = 0f;
    protected float FireRateUpgradePercent = 0.2f;

    protected int BaseDamage = 25;
    protected int DamageUpgradeFlat = 5;
    protected float DamageUpgradePercent = 0.1f;

    public float CurrentRange => (BaseRange + (CurrentLevel * RangeUpgradeFlat)) * (1f + (CurrentLevel * RangeUpgradePercent));
    public float CurrentFireRate => (BaseFireRate + (CurrentLevel * FireRateUpgradeFlat)) * (1f + (CurrentLevel * FireRateUpgradePercent));
    public int CurrentDamage => (int)((BaseDamage + (CurrentLevel * DamageUpgradeFlat)) * (1f + (CurrentLevel * DamageUpgradePercent)));

    public float ProjectileSpeed { get; set; } = 200f;
    public AttackMode AttackMode { get; set; } = AttackMode.Closest;
    public float RotationSpeed { get; set; } = 5f; // radians per second

    public int BaseUpgradeCost { get; set; } = 50;

    public int GetUpgradeCost() 
    {
        return (int)(BaseUpgradeCost * Math.Pow(1.5, CurrentLevel));
    }

    public bool CanUpgrade => CurrentLevel < MaxLevel;

    public virtual void ApplyUpgrade()
    {
        if (CanUpgrade)
        {
            CurrentLevel++;
        }
    }

    private float _fireCooldown = 0f;
    private float _targetRotation = 0f;

    public BaseTower(Vector2 location, List<Troop> troops, string texturePath)
    {
        Position = location;
        Troops = troops;

        Texture = RumGame.Instance.Content.Load<Texture2D>(texturePath);
        origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
        rotationOffset = MathHelper.Pi;

        Size = SizeSystem.Square(1f);
        ApplySize();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // update projectiles
        for (int i = Projectiles.Count - 1; i >= 0; i--)
        {
            Projectiles[i].Update(gameTime);
            if (Projectiles[i].IsFinished)
                Projectiles.RemoveAt(i);
        }

        // find target every frame so rotation stays smooth
        Troop target = FindTarget();

        // rotate toward target
        if (target != null)
        {
            Vector2 dir = target.Position - Position;
            _targetRotation = (float)Math.Atan2(dir.Y, dir.X);
        }
        float diff = MathHelper.WrapAngle(_targetRotation - rotation);
        rotation += diff * Math.Min(1f, RotationSpeed * dt);

        // handle firing
        _fireCooldown -= dt;
        if (_fireCooldown > 0f) return;
        if (target == null) return;

        FireProjectile(target);
        _fireCooldown = 1f / CurrentFireRate;
    }

    protected virtual void FireProjectile(Troop target)
    {
        AudioManager.Instance.PlaySound("shoot");
        Projectiles.Add(new Projectile(Position, target, ProjectileSpeed, CurrentDamage));
    }

    private Troop FindTarget()
    {
        Troop best = null;
        float bestValue = float.MaxValue;

        foreach (var troop in Troops)
        {
            if (troop.IsDead || troop.IsFinished) continue;

            float dist = Vector2.Distance(Position, troop.Position);
            if (dist > CurrentRange) continue;

            float value = AttackMode switch
            {
                AttackMode.Closest => dist,
                AttackMode.Strongest => -troop.Health, // highest HP = lowest value
                AttackMode.First => (troop.Path != null && troop.Path.Count > 0 ? (troop.Path.Count * 1000f) + Vector2.Distance(troop.Position, troop.Path.Peek()) : dist),
                _ => dist
            };

            if (value < bestValue)
            {
                bestValue = value;
                best = troop;
            }
        }

        return best;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        foreach (var proj in Projectiles)
            proj.Draw(spriteBatch);
    }
}
