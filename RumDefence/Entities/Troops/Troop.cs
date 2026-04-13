using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence;

public class Troop : EntityWithHealth
{
    private Vector2 target;
    protected virtual Animation animation {get; set; }

    private float baseSpeed = 60f;
    public float SpeedMultiplier { get; set; } = 1f;
    public int CoinValue { get; set; } = 1;
    public bool HasDroppedReward { get; private set; }
    public bool IsFinished { get; private set; }

    private List<ITroopAbility> abilities = new();

    public Troop(string spritePath, Vector2 start, Vector2 targetPos) : base(16, 32)
    {
        Position = start;
        target = targetPos;
        animation = new TroopAnimation(
            16,
            16,
            0.2f,
            3,
            true
        );

        // https://foozlecc.itch.io/scallywag-pirates
        Texture = RumGame.Instance.Content.Load<Texture2D>(spritePath);
        origin = Vector2.Zero;

        Size = SizeSystem.Square(10f);
        ApplySize();
    }

    public void AddAbility(ITroopAbility ability)
    {
        abilities.Add(ability);
    }

    public override void Update(GameTime gameTime)
    {
        if (IsFinished || IsDead) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        SpeedMultiplier = 1f;

        foreach (var ability in abilities)
        {
            ability.Update(this, gameTime);
        }

        float speed = baseSpeed * SpeedMultiplier;

        Vector2 dir = target - Position;

        if (dir.Length() < 5f)
        {
            IsFinished = true;
            return;
        }

        dir.Normalize();
        sourceRectangles = animation.GetCurrentLayerRectangles(gameTime, dir);

        Position += dir * speed * dt;
    }
    public void MarkRewardGiven()
    {
        HasDroppedReward = true;
    }
}