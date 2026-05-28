using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public enum SpriteAction
{
    None,
    Static,
    Idle,
    Walking,
    Attack,
    Dying,
    Rotation,
}
public enum SpriteDirection
{
    None,
    Down,
    Up,
    Right,
    Left,
}
public readonly struct SpriteLayer
{
    public SpriteAction Type { get; }
    public int StartColumn { get; }
    public int EndColumn { get; }
    public int Row { get; }
    public float Depth { get; }
    public readonly int TotalFrames => EndColumn - StartColumn + 1;
    public SpriteEffects Effect { get; }
    public SpriteDirection Direction { get; }
    public SpriteLayer(
        int startColumn, int endColumn, int row,
        SpriteAction type = SpriteAction.None, SpriteDirection direction = SpriteDirection.None,
        float depth = 0f, SpriteEffects effect = SpriteEffects.None
    )
    {
        Type = type;
        StartColumn = startColumn;
        EndColumn = endColumn;
        Row = row;
        Depth = depth;
        Effect = effect;
        Direction = direction;
    }
}

public readonly struct SpriteMatrixCell
{
    public SpriteDirection Direction { get; }
    public SpriteAction Action { get; }
    public SpriteEffects Effect { get; }
    public int Frames { get; }
    public bool IsLoop { get; }
    public SpriteMatrixCell(
        int frames = 1,
        SpriteAction action = SpriteAction.None,
        SpriteDirection direction = SpriteDirection.None,
        SpriteEffects effect = SpriteEffects.None,
        bool isLoop = true
    )
    {
        Direction = direction;
        Action = action;
        Frames = frames;
        Effect = effect;
        IsLoop = isLoop;
    }
}

public interface IAnimation
{
    int FrameWidth { get; set; }
    int FrameHeight { get; set; }
    float FrameDuration { get; set; }
    bool HasAnimation { get; }
    float ElapseTime { get; set; }
    int CurrentFrame { get; set; }
    bool IsFinished { get; set; }
    void Update(GameTime gameTime);
    Tuple<SpriteLayer, Rectangle>[] GetCurrentLayers();
}

public class Animation : IAnimation
{
    public Texture2D Texture { get; set; }
    public int FrameWidth { get; set; }
    public int FrameHeight { get; set; }
    public float FrameDuration { get; set; }
    public bool HasAnimation { get; } = false;
    public int ColumnOffset { get; set; } = 0;
    public float ElapseTime { get; set; }
    public int CurrentFrame { get; set; }
    public bool IsFinished { get; set; }

    private SpriteMatrixCell[,] SpriteMatrices { get; set; }
    private Tuple<SpriteAction, SpriteDirection>[] ActiveSpriteLayer { get; set; } = [];

    public Animation()
    {
        HasAnimation = false;
    }
    public Animation(int frameWidth, int frameHeight, float frameDuration)
    {
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
        FrameDuration = frameDuration;
        HasAnimation = true;
    }
    public Animation(Texture2D texture, int frameWidth, int frameHeight, float frameDuration)
    {
        Texture = texture;
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
        FrameDuration = frameDuration;
        HasAnimation = true;
    }

    public void AddLayerMatrix(SpriteMatrixCell[,] matrixCells)
    {
        if (SpriteMatrices == null) { SpriteMatrices = matrixCells; return; }
        int existingRows = SpriteMatrices.GetLength(0);
        int addedRows = matrixCells.GetLength(0);
        int cols = matrixCells.GetLength(1);
        var merged = new SpriteMatrixCell[existingRows + addedRows, cols];
        for (int r = 0; r < existingRows; r++)
            for (int c = 0; c < SpriteMatrices.GetLength(1); c++)
                merged[r, c] = SpriteMatrices[r, c];
        for (int r = 0; r < addedRows; r++)
            for (int c = 0; c < cols; c++)
                merged[existingRows + r, c] = matrixCells[r, c];
        SpriteMatrices = merged;
    }

    public void AddLayerMatrix(SpriteMatrixCell[] row, int rowCount = 1)
    {
        var matrix = new SpriteMatrixCell[rowCount, row.Length];
        for (int r = 0; r < rowCount; r++)
            for (int c = 0; c < row.Length; c++)
                matrix[r, c] = row[c];
        AddLayerMatrix(matrix);
    }

    public void ActivateLayers(Tuple<SpriteAction, SpriteDirection>[] spriteLayers)
    {
        if (!LayersEqual(ActiveSpriteLayer, spriteLayers))
        {
            ElapseTime = 0;
            CurrentFrame = 0;
            IsFinished = false;
        }
        ActiveSpriteLayer = spriteLayers;
    }

    public void Update(GameTime gameTime)
    {
        ElapseTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (FrameDuration <= 0) return;
        CurrentFrame = (int)(ElapseTime / FrameDuration);
    }

    public virtual Tuple<SpriteLayer, Rectangle>[] GetCurrentLayers(GameTime gameTime)
    {
        Update(gameTime);
        return GetCurrentLayers();
    }

    public virtual Tuple<SpriteLayer, Rectangle>[] GetCurrentLayers()
    {
        if (SpriteMatrices == null) return [];

        int rows = SpriteMatrices.GetLength(0);
        int cols = SpriteMatrices.GetLength(1);
        var results = new List<Tuple<SpriteLayer, Rectangle>>();

        for (int r = 0; r < rows; r++)
        {
            int colOffset = ColumnOffset;
            for (int c = 0; c < cols; c++)
            {
                var cell = SpriteMatrices[r, c];
                if (IsLayerActive(cell.Action, cell.Direction))
                {
                    int frame;
                    if (cell.IsLoop)
                        frame = cell.Frames > 0 ? CurrentFrame % cell.Frames : 0;
                    else
                    {
                        frame = cell.Frames > 0 ? Math.Min(CurrentFrame, cell.Frames - 1) : 0;
                        if (CurrentFrame >= cell.Frames)
                            IsFinished = true;
                    }
                    var rect = new Rectangle(
                        (colOffset + frame) * FrameWidth,
                        r * FrameHeight,
                        FrameWidth,
                        FrameHeight
                    );
                    var layer = new SpriteLayer(
                        colOffset, colOffset + cell.Frames - 1, r,
                        cell.Action, cell.Direction,
                        r * 0.001f, cell.Effect
                    );
                    results.Add(Tuple.Create(layer, rect));
                }
                colOffset += cell.Frames;
            }
        }

        // Deferred sort mode uses draw order, not depth. Reverse so row 0 is drawn last = on top.
        results.Reverse();
        return results.ToArray();
    }

    private bool IsLayerActive(SpriteAction action, SpriteDirection direction)
    {
        if (ActiveSpriteLayer == null || ActiveSpriteLayer.Length == 0) return false;
        foreach (var pair in ActiveSpriteLayer)
            if (pair.Item1 == action && pair.Item2 == direction)
                return true;
        return false;
    }

    private static bool LayersEqual(Tuple<SpriteAction, SpriteDirection>[] a, Tuple<SpriteAction, SpriteDirection>[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
            if (a[i].Item1 != b[i].Item1 || a[i].Item2 != b[i].Item2) return false;
        return true;
    }
}
