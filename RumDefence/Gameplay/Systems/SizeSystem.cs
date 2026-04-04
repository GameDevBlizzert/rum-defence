using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public static class SizeSystem
{
    /// <summary>
    /// Zet tile-based grootte om naar world size (pixels)
    /// </summary>
    public static Vector2 FromTiles(float widthInTiles, float heightInTiles)
    {
        float tileSize = RumGame.Instance.CurrentGrid.TileSize;

        return new Vector2(
            widthInTiles * tileSize,
            heightInTiles * tileSize
        );
    }

    /// <summary>
    /// Shortcut voor vierkante entities (meest gebruikt)
    /// </summary>
    public static Vector2 Square(float sizeInTiles)
    {
        return FromTiles(sizeInTiles, sizeInTiles);
    }

    /// <summary>
    /// Zet gewenste world size om naar scale voor rendering
    /// </summary>
    public static float ToScale(Texture2D texture, Vector2 size)
    {
        if (texture == null || texture.Width == 0)
            return 1f;

        return size.X / texture.Width;
    }

    /// <summary>
    /// Combineert size + multiplier (handig voor fine tuning)
    /// </summary>
    public static float ToScaled(Texture2D texture, Vector2 size, float multiplier)
    {
        return ToScale(texture, size) * multiplier;
    }
}