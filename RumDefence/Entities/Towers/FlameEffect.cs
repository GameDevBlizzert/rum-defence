using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class FlameEffect : Entity
{
    private const int FrameWidth = 128;
    private const int FrameHeight = 64;
    private const int NumFrames = 8;
    private const float FrameDuration = 0.1f;
    public const float TotalDuration = NumFrames * FrameDuration;
    private const float SpreadAngle = MathHelper.Pi / 12f;
    private const float FlameHeight = 60f;

    private static readonly float ConeTan = MathF.Tan(SpreadAngle);
    private static readonly float ConeOffset = FlameHeight / 2f * MathF.Cos(SpreadAngle);

    private readonly float _maxRange;
    private readonly float _damage;
    private readonly Vector2 _fireDir;
    private readonly float _centerRot;
    private readonly float _innerLeftRot;
    private readonly float _innerRightRot;
    private readonly float _outerLeftRot;
    private readonly float _outerRightRot;
    private readonly Vector2 _drawScale;
    private readonly Vector2 _spriteOrigin;
    private readonly Animation _animation;
    private readonly HashSet<Troop> _hitTroops = [];
    private float _elapsed;

    public bool IsFinished { get; private set; }

    public FlameEffect(Vector2 towerPos, Vector2 direction, float maxRange, float damage)
    {
        Position = towerPos;
        _maxRange = maxRange;
        _damage = damage;
        _fireDir = direction;

        float baseAngle = MathF.Atan2(direction.Y, direction.X) + MathF.PI;
        _centerRot = baseAngle;
        _innerLeftRot = baseAngle - SpreadAngle / 2f;
        _innerRightRot = baseAngle + SpreadAngle / 2f;
        _outerLeftRot = baseAngle - SpreadAngle;
        _outerRightRot = baseAngle + SpreadAngle;

        Texture = RumGame.Instance.Content.Load<Texture2D>("Art/Projectiles/flames");

        _spriteOrigin = new Vector2(FrameWidth, FrameHeight / 2f);
        _drawScale = new Vector2(maxRange / FrameWidth, FlameHeight / FrameHeight);

        _animation = new Animation(FrameWidth, FrameHeight, FrameDuration);
        _animation.AddLayerMatrix([new(NumFrames, SpriteAction.Static, SpriteDirection.None)]);
        _animation.ActivateLayers([new(SpriteAction.Static, SpriteDirection.None)]);
    }

    public override void Update(GameTime gameTime)
    {
        if (IsFinished) return;

        _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        _animation.Update(gameTime);

        int frame = Math.Min(_animation.CurrentFrame, NumFrames - 1);
        float currentReach = (float)(frame + 1) / NumFrames * _maxRange;

        foreach (var troop in GameScreen.Instance.Troops)
        {
            if (troop.IsDead || troop.IsFinished || _hitTroops.Contains(troop)) continue;

            Vector2 toTroop = troop.Position - Position;
            float along = Vector2.Dot(toTroop, _fireDir);
            if (along <= 0f || along > currentReach) continue;

            float perp = MathF.Abs(toTroop.X * _fireDir.Y - toTroop.Y * _fireDir.X);
            if (perp > along * ConeTan + ConeOffset) continue;

            troop.TakeDamage(_damage);
            _hitTroops.Add(troop);
        }

        const float fadeStart = TotalDuration * 0.6f;
        float alpha = _elapsed < fadeStart
            ? 1f
            : 1f - (_elapsed - fadeStart) / (TotalDuration - fadeStart);
        color = Color.White * MathF.Max(0f, alpha);

        if (_elapsed >= TotalDuration)
            IsFinished = true;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var layers = _animation.GetCurrentLayers();
        foreach (var layer in layers)
        {
            spriteBatch.Draw(Texture, Position, layer.Item2, color * 0.55f, _outerLeftRot, _spriteOrigin, _drawScale, layer.Item1.Effect, layer.Item1.Depth);
            spriteBatch.Draw(Texture, Position, layer.Item2, color * 0.55f, _outerRightRot, _spriteOrigin, _drawScale, layer.Item1.Effect, layer.Item1.Depth);
            spriteBatch.Draw(Texture, Position, layer.Item2, color * 0.78f, _innerLeftRot, _spriteOrigin, _drawScale, layer.Item1.Effect, layer.Item1.Depth);
            spriteBatch.Draw(Texture, Position, layer.Item2, color * 0.78f, _innerRightRot, _spriteOrigin, _drawScale, layer.Item1.Effect, layer.Item1.Depth);
            spriteBatch.Draw(Texture, Position, layer.Item2, color, _centerRot, _spriteOrigin, _drawScale, layer.Item1.Effect, layer.Item1.Depth);
        }
    }
}
