using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence;

public class FantasyPanel
{
    private Texture2D _borderTexture;
    private readonly int _borderStyle;
    private const int PATCH_SIZE = 32; // Each region in the 96x96 texture is 32x32
    private Rectangle _bounds;
    private Color _tint = Color.White;
    private bool _drawCenterFill = true;

    public bool DrawCenterFill
    {
        get => _drawCenterFill;
        set => _drawCenterFill = value;
    }

    public Color Tint
    {
        get => _tint;
        set => _tint = value;
    }

    public FantasyPanel(int borderStyle = 0)
    {
        _borderStyle = Math.Clamp(borderStyle, 0, 31);
        LoadBorderTexture();
    }

    private void LoadBorderTexture()
    {
        string stylePath = $"Art/UI/FantasyKit/Border/panel-border-{_borderStyle:D3}";
        try
        {
            _borderTexture = RumGame.Instance.Content.Load<Texture2D>(stylePath);
        }
        catch
        {
            // Fallback to first border if style doesn't exist
            _borderTexture = RumGame.Instance.Content.Load<Texture2D>("Art/UI/FantasyKit/Border/panel-border-000");
        }
    }

    public void SetBounds(Rectangle bounds)
    {
        _bounds = bounds;
    }

    public void SetBounds(int x, int y, int width, int height)
    {
        _bounds = new Rectangle(x, y, width, height);
    }

    public Rectangle GetContentArea()
    {
        return new Rectangle(
            _bounds.X + PATCH_SIZE,
            _bounds.Y + PATCH_SIZE,
            Math.Max(0, _bounds.Width - PATCH_SIZE * 2),
            Math.Max(0, _bounds.Height - PATCH_SIZE * 2)
        );
    }

    public Rectangle GetBounds() => _bounds;

    public void Draw(SpriteBatch spriteBatch, float layerDepth = 0f)
    {
        if (_borderTexture == null) return;

        int w = _bounds.Width;
        int h = _bounds.Height;
        int x = _bounds.X;
        int y = _bounds.Y;

        // 9-patch layout: divide 96x96 into 9 equal 32x32 regions
        // 0,0 | 32,0 | 64,0
        // 0,32| 32,32| 64,32
        // 0,64| 32,64| 64,64

        int ps = PATCH_SIZE;

        // Corners (fixed, no tiling)
        DrawPatch(spriteBatch, x, y, ps, ps, 0, 0, ps, ps, layerDepth);                                    // Top-left
        DrawPatch(spriteBatch, x + w - ps, y, ps, ps, ps * 2, 0, ps, ps, layerDepth);                      // Top-right
        DrawPatch(spriteBatch, x, y + h - ps, ps, ps, 0, ps * 2, ps, ps, layerDepth);                      // Bottom-left
        DrawPatch(spriteBatch, x + w - ps, y + h - ps, ps, ps, ps * 2, ps * 2, ps, ps, layerDepth);        // Bottom-right

        // Edges (tiled, not stretched)
        if (w > ps * 2)
        {
            // Top edge - tile horizontally
            int edgeWidth = w - ps * 2;
            int edgeX = x + ps;
            TileHorizontal(spriteBatch, edgeX, y, edgeWidth, ps, ps, 0, ps, ps, layerDepth);

            // Bottom edge - tile horizontally
            TileHorizontal(spriteBatch, edgeX, y + h - ps, edgeWidth, ps, ps, ps * 2, ps, ps, layerDepth);
        }

        if (h > ps * 2)
        {
            // Left edge - tile vertically
            int edgeHeight = h - ps * 2;
            int edgeY = y + ps;
            TileVertical(spriteBatch, x, edgeY, ps, edgeHeight, 0, ps, ps, ps, layerDepth);

            // Right edge - tile vertically
            TileVertical(spriteBatch, x + w - ps, edgeY, ps, edgeHeight, ps * 2, ps, ps, ps, layerDepth);
        }

        // Center fill (only if enabled)
        if (_drawCenterFill && w > ps * 2 && h > ps * 2)
        {
            int centerWidth = w - ps * 2;
            int centerHeight = h - ps * 2;
            int centerX = x + ps;
            int centerY = y + ps;
            TileBoth(spriteBatch, centerX, centerY, centerWidth, centerHeight, ps, ps, ps, ps, layerDepth);
        }
    }

    private void TileHorizontal(SpriteBatch spriteBatch, int destX, int destY, int destW, int destH, int srcX, int srcY, int srcW, int srcH, float layerDepth)
    {
        for (int i = 0; i < destW; i += srcW)
        {
            int w = Math.Min(srcW, destW - i);
            DrawPatch(spriteBatch, destX + i, destY, w, destH, srcX, srcY, w, srcH, layerDepth);
        }
    }

    private void TileVertical(SpriteBatch spriteBatch, int destX, int destY, int destW, int destH, int srcX, int srcY, int srcW, int srcH, float layerDepth)
    {
        for (int i = 0; i < destH; i += srcH)
        {
            int h = Math.Min(srcH, destH - i);
            DrawPatch(spriteBatch, destX, destY + i, destW, h, srcX, srcY, srcW, h, layerDepth);
        }
    }

    private void TileBoth(SpriteBatch spriteBatch, int destX, int destY, int destW, int destH, int srcX, int srcY, int srcW, int srcH, float layerDepth)
    {
        for (int yi = 0; yi < destH; yi += srcH)
        {
            for (int xi = 0; xi < destW; xi += srcW)
            {
                int w = Math.Min(srcW, destW - xi);
                int h = Math.Min(srcH, destH - yi);
                DrawPatch(spriteBatch, destX + xi, destY + yi, w, h, srcX, srcY, w, h, layerDepth);
            }
        }
    }

    private void DrawPatch(SpriteBatch spriteBatch, int destX, int destY, int destW, int destH, int srcX, int srcY, int srcW, int srcH, float layerDepth)
    {
        if (destW <= 0 || destH <= 0) return;

        var destRect = new Rectangle(destX, destY, destW, destH);
        var srcRect = new Rectangle(srcX, srcY, srcW, srcH);

        spriteBatch.Draw(_borderTexture, destRect, srcRect, _tint, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
    }
}
