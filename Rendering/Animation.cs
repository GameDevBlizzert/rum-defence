using Microsoft.Xna.Framework;

namespace Rum_Defence.Rendering;

public struct SpriteLayer
{
    public int StartColumn;
    public int EndColumn;
    public int Row;

    public SpriteLayer(int startColumn, int endColumn, int row)
    {
        StartColumn = startColumn;
        EndColumn = endColumn;
        Row = row;
    }
    public Rectangle GetSourceRectangle(int animationFrame, int frameWidth, int frameHeight)
    {
        int totalFramesInLayer = (EndColumn - StartColumn) + 1;
        int column = StartColumn + (animationFrame % totalFramesInLayer);

        return new Rectangle(column * frameWidth, Row * frameHeight, frameWidth, frameHeight);
    }
}

public abstract class Animation
{
    public int FrameWidth { get; protected set; }
    public int FrameHeight { get; protected set; }
    public int FrameTotal { get; protected set; }
    public float FrameDuration { get; protected set; }
    public bool IsLoop { get; protected set; }
    public float ElapseTime { get; protected set; }
    public int CurrentFrame { get; protected set; }

    public Animation(int frameWidth, int frameHeight, float frameDuration, int totalFrames, bool isLoop)
    {
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
        FrameDuration = frameDuration;
        FrameTotal = totalFrames;
        IsLoop = isLoop;
    }

    public abstract Rectangle[] GetCurrentLayerRectangles(GameTime gameTime, Vector2 direction);
}