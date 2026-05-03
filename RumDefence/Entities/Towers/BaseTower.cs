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

    public float Range { get; set; } = 700f;
    public float FireRate { get; set; } = 1f; // shots per second
    public int Damage { get; set; } = 25;
    public float ProjectileSpeed { get; set; } = 200f;
    public AttackMode AttackMode { get; set; } = AttackMode.Closest;
    public float RotationSpeed { get; set; } = 5f; // radians per second

    private float _fireCooldown = 0f;
    private float _targetRotation = 0f;

    public BaseTower(TowerData data, Vector2 location, List<Troop> troops)
    {
        Position = location;
        Troops = troops;

        Range = data.Range;
        FireRate = data.FireRate;
        Damage = data.Damage;
        ProjectileSpeed = data.ProjectileSpeed;
        AttackMode = data.AttackMode;

        Texture = RumGame.Instance.Content.Load<Texture2D>(data.TexturePath);
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
        _fireCooldown = 1f / FireRate;
    }

    protected virtual void FireProjectile(Troop target)
    {
        Projectiles.Add(new Projectile(Position, target, ProjectileSpeed, Damage));
    }

    private Troop FindTarget()
    {
        Troop best = null;
        float bestValue = float.MaxValue;

        foreach (var troop in Troops)
        {
            if (troop.IsDead || troop.IsFinished) continue;

            float dist = Vector2.Distance(Position, troop.Position);
            if (dist > Range) continue;

            float value = AttackMode switch
            {
                AttackMode.Closest => dist,
                AttackMode.Strongest => -troop.Health, // highest HP = lowest value
                AttackMode.First => dist,          // TODO: replace with path progress location of the rum
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
