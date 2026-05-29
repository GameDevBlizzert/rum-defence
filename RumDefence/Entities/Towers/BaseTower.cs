using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class BaseTower : Entity
{
    protected readonly List<Troop> Troops;
    protected readonly List<BaseProjectile> Projectiles = [];

    public TowerData Data { get; }

    public int CurrentLevel { get; protected set; } = 0;
    public int MaxLevel { get; protected set; } = 3;

    public string Label = "";

    public float CurrentRange => (Data.Range + (CurrentLevel * Data.RangeUpgradeFlat)) * (1f + (CurrentLevel * Data.RangeUpgradePercent));
    public float CurrentFireRate => (Data.FireRate + (CurrentLevel * Data.FireRateUpgradeFlat)) * (1f + (CurrentLevel * Data.FireRateUpgradePercent));
    public int CurrentDamage => (int)((Data.Damage + (CurrentLevel * Data.DamageUpgradeFlat)) * (1f + (CurrentLevel * Data.DamageUpgradePercent)));

    public float ProjectileSpeed { get; set; } = 200f;
    public AttackMode AttackMode { get; set; } = AttackMode.Closest;
    public float RotationSpeed { get; set; } = 5f;

    public int GetUpgradeCost()
    {
        return (int)(Data.UpgradeCost * Math.Pow(1.5, CurrentLevel));
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
    private float _targetRotation = MathHelper.Pi;
    private SpriteDirection _lastFacingDir = SpriteDirection.Down;
    protected readonly Animation animation;

    public BaseTower(TowerData data, Vector2 location, List<Troop> troops)
    {
        Data = data;
        Position = location;
        Troops = troops;

        ProjectileSpeed = data.ProjectileSpeed;
        AttackMode = data.AttackMode;
        Label = data.Label;

        Texture = RumGame.Instance.Content.Load<Texture2D>(data.TexturePath);
        rotationOffset = MathHelper.Pi;

        Size = SizeSystem.Square(Primitives.TowerSize) * data.ScaleMultiplier;
        animation = new(Texture, 64, 64, 1f);
        origin = new Vector2(animation.FrameWidth / 2f, animation.FrameHeight / 2f);
        scale = Size.X / animation.FrameWidth;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        for (int i = Projectiles.Count - 1; i >= 0; i--)
        {
            Projectiles[i].Update(gameTime);
            if (Projectiles[i].IsFinished)
                Projectiles.RemoveAt(i);
        }

        Troop target = FindTarget();

        Vector2 dir = Vector2.Zero;
        if (target != null)
        {
            dir = target.Position - Position;
            _targetRotation = (float)Math.Atan2(dir.Y, dir.X);
        }
        float diff = MathHelper.WrapAngle(_targetRotation - rotation);
        rotation += diff * Math.Min(1f, RotationSpeed * dt);

        if (dir != Vector2.Zero)
            _lastFacingDir = GetFacingDirection();
        var facingDir = _lastFacingDir;
        animation.ActivateLayers([
            new(SpriteAction.Rotation, facingDir),
            new(SpriteAction.Static, facingDir),
        ]);

        _fireCooldown -= dt;

        animation.Update(gameTime);

        if (_fireCooldown > 0f) return;
        if (target == null) return;

        FireProjectile(target);
        _fireCooldown = 1f / CurrentFireRate;
    }

    protected virtual void FireProjectile(Troop target)
    {
        AudioManager.Instance.PlaySound("shoot", maxConcurrentInstances: 4);
        Projectiles.Add(new BulletProjectile(Position, target, ProjectileSpeed, CurrentDamage));
    }

    private float GetPendingDamage(Troop troop)
    {
        float pending = 0;
        foreach (var proj in Projectiles)
            if (proj.Target == troop)
                pending += proj.Damage;
        return pending;
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

            if (troop.Health - GetPendingDamage(troop) <= 0) continue;

            float value = AttackMode switch
            {
                AttackMode.Closest => dist,
                AttackMode.Strongest => -troop.Health,
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
        DrawSpriteLayers(spriteBatch);
        DrawProjectiles(spriteBatch);
        DrawLevelStripes(spriteBatch);
    }
    protected SpriteDirection GetFacingDirection()
    {
        float angle = _targetRotation;
        if (angle > -MathHelper.PiOver4 && angle <= MathHelper.PiOver4) return SpriteDirection.Right;
        if (angle > MathHelper.PiOver4 && angle <= 3 * MathHelper.PiOver4) return SpriteDirection.Down;
        if (angle > -3 * MathHelper.PiOver4 && angle <= -MathHelper.PiOver4) return SpriteDirection.Up;
        return SpriteDirection.Left;
    }

    public void DrawSpriteLayers(SpriteBatch spriteBatch)
    {
        var items = animation.GetCurrentLayers();
        foreach (var item in items)
        {
            float itemRotation = item.Item1.Type == SpriteAction.Rotation ? rotation + rotationOffset : 0f;
            spriteBatch.Draw(
                Texture,
                Position,
                item.Item2,
                color,
                itemRotation,
                origin,
                scale,
                item.Item1.Effect,
                item.Item1.Depth
            );
        }
    }
    public virtual void DrawProjectiles(SpriteBatch spriteBatch)
    {
        foreach (var proj in Projectiles)
            proj.Draw(spriteBatch);
    }

    public virtual void DrawLevelStripes(SpriteBatch spriteBatch)
    {
        var gapBetweenStripes = new Vector2(6, 0);
        var rect = new Rectangle(0, 0, 6, 6);
        var levelStripeOrigin = rect.Center.ToVector2();
        Color color;
        Vector2 levelStripeSize = (gapBetweenStripes * MaxLevel) + rect.Size.ToVector2() * (MaxLevel + 1) * new Vector2(1, 0);
        var levelStripePos = Position - levelStripeSize / 2 - new Vector2(0, Size.Y / 2);
        for (int i = 0; i <= MaxLevel; i++)
        {
            if (i <= CurrentLevel)
                color = Color.Yellow;
            else
                color = Color.DarkRed;

            spriteBatch.Draw(
                Primitives.Pixel,
                levelStripePos + new Vector2(rect.Width, 0) * i + gapBetweenStripes * i,
                rect,
                color,
                0f,
                levelStripeOrigin,
                1f,
                SpriteEffects.None,
                layerDepth + 0.1f
            );
        }
    }
}
