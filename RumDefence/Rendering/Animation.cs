using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace RumDefence;

public enum SpriteType
{
    Down,
    Up,
    Right,
    Left,
    Attack,
    Dying,
    Special,
    Idle,
}
public struct SpriteLayer
{
    public SpriteType Type;
    public int StartColumn;
    public int EndColumn;
    public int Row;
    public float Depth;
    public readonly int TotalFrames => EndColumn - StartColumn + 1;
    public SpriteLayer(SpriteType type, int startColumn, int endColumn, int row, float depth = 1)
    {
        Type = type;
        StartColumn = startColumn;
        EndColumn = endColumn;
        Row = row;
        Depth = depth;
    }
    public Rectangle GetSourceRectangle(int animationFrame, int frameWidth, int frameHeight)
    {
        int column = StartColumn + (animationFrame % TotalFrames);
        return new Rectangle(column * frameWidth, Row * frameHeight, frameWidth, frameHeight);
    }
}

public abstract class Animation
{
    public int FrameWidth { get; protected set; }
    public int FrameHeight { get; protected set; }
    public int FrameTotal => _ActiveLayers?.Length > 0 ? _ActiveLayers[0].TotalFrames : 0;
    public float FrameDuration { get; protected set; }
    public bool IsLoop { get; protected set; }
    public float ElapseTime { get; protected set; }
    public int CurrentFrame { get; protected set; }
    public bool IsFinished { get; protected set; }
    private List<SpriteLayer> _allLayers = new();
    private SpriteLayer[] _ActiveLayers;
    protected SpriteLayer[] _defaultlayers;

    public Animation(int frameWidth, int frameHeight, float frameDuration, bool isLoop = false)
    {
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
        FrameDuration = frameDuration;
        IsLoop = isLoop;
    }
    public void AddSpriteLayer(SpriteLayer layer)
    {
        _allLayers.Add(layer);
    }
    protected void ClearLayers()
    {
        _allLayers.Clear();
    }
    public void Update(GameTime gameTime)
    {
        ElapseTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        CurrentFrame = (int)(ElapseTime / FrameDuration);
        ResetLoop();
    }
    public void ResetLoop()
    {
        if (!IsLoop && FrameTotal > 0)
        {
            if (CurrentFrame >= FrameTotal)
            {
                CurrentFrame = FrameTotal - 1;
                IsFinished = true;
            }
        }
    }
    public virtual Rectangle[] GetCurrentLayerRectangles(GameTime gameTime)
    {
        Update(gameTime);
        return GetCurrentLayerRectangles();
    }
    public virtual Rectangle[] GetCurrentLayerRectangles()
    {
        if (_ActiveLayers == null)
            return default;

        Rectangle[] rects = new Rectangle[_ActiveLayers.Length];
        for (int i = 0; i < _ActiveLayers.Length; i++)
        {
            rects[i] = _ActiveLayers[i].GetSourceRectangle(CurrentFrame, FrameWidth, FrameHeight);
        }
        return rects;
    }

    public void UpdateActiveLayers(SpriteType type)
    {
        _ActiveLayers = _allLayers.Where(l => l.Type == type).ToArray();
    }
}
