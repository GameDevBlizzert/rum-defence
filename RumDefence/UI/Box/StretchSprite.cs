using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence.UI.Box;

public static class StretchSprite
{
    public static Rectangle[] GetCorners(Rectangle rect, Point size)
    {
        var TopLeftCornerSource = new Rectangle(rect.Left, rect.Top, size.X, size.Y);
        var TopRightCornerSource = new Rectangle(rect.Right - size.X, rect.Top, size.X, size.Y);
        var BottomLeftCornerSource = new Rectangle(rect.Left, rect.Bottom - size.Y, size.X, size.Y);
        var BottomRightCornerSource = new Rectangle(rect.Right - size.X, rect.Bottom - size.Y, size.X, size.Y);
        return [TopLeftCornerSource, TopRightCornerSource, BottomLeftCornerSource, BottomRightCornerSource];
    }
    // upgrade voor NineSlice voor crispy, pixel perfect, OCD statisfying tekenen van een vierkante texture.
    // hiermee hoef je voor elke rechthoek geen nieuwe sprite te maken.
    // een vierkant wordt gesplits in 9 stukken.
    // de hoeken zijn statish en de edges en center worden wel gestrekt.
    public static void Draw(SpriteBatch spriteBatch, Texture2D texture, Rectangle destination, Rectangle? source = null, Color? color = null)
    {
        var _color = color ?? Color.White;
        // dit werkt beter als de texture vierkant is
        var _source = source ?? new Rectangle(0, 0, texture.Width, texture.Height);
        // splits source rectangle in 9 tiles door X en Y te delen met 3.
        var srcTileSize = (_source.Size.ToVector2() / 3).ToPoint();

        // backup als destination veels te klein is.
        // if (destination.Width < _source.Width && destination.Height < _source.Height)
        // {
        //     spriteBatch.Draw(texture, destination, _source, _color);
        //     return;
        // }

        // pak de minimale width en height van de texture voor de hoeken. 
        // soms is destination kleiner dan de texture dus pak je dan daarvan de 1/2 in plaats 1/3 texture.
        int cornerW = Math.Min(srcTileSize.X, destination.Width / 2);
        int cornerH = Math.Min(srcTileSize.Y, destination.Height / 2);

        // hoekpunten (vervangt de GetCorners functie)
        // destination:
        // [links x1, links x2, rechts x1, rechts x2]
        int[] destXs = [destination.Left, destination.Left + cornerW, destination.Right - cornerW, destination.Right];
        // [boven y1, boven y2, beneden y1, beneden y2]
        int[] destYs = [destination.Top, destination.Top + cornerH, destination.Bottom - cornerH, destination.Bottom];

        // source:
        // [links x1, links x2, rechts x1, rechts x2]
        int[] srcXs = [_source.Left, _source.Left + srcTileSize.X, _source.Right - srcTileSize.X, _source.Right];
        // [boven y1, boven y2, beneden y1, beneden y2]
        int[] srcYs = [_source.Top, _source.Top + srcTileSize.Y, _source.Bottom - srcTileSize.Y, _source.Bottom];

        for (int x = 0; x < 3; x++)
        {
            int destX = destXs[x];
            // De hoeken zijn vast en alleen het middelste wordt gestrekt. x[2] - x[1]
            // todo: in plaats van strekken invullen met rechthoeken (moeilijk).
            int destW = destXs[x + 1] - destXs[x];
            int srcX = srcXs[x];
            int srcW = srcXs[x + 1] - srcXs[x];

            for (int y = 0; y < 3; y++)
            {
                int destY = destYs[y];
                // De hoeken zijn vast en alleen het middelste wordt gestrekt. y[2] - y[1]
                // todo: in plaats van strekken invullen met rechthoeken (moeilijk).
                int destH = destYs[y + 1] - destYs[y];
                int srcY = srcYs[y];
                int srcH = srcYs[y + 1] - srcYs[y];

                if (destW <= 0 || destH <= 0)
                    continue;

                var destRect = new Rectangle(destX, destY, destW, destH);
                var sourceRect = new Rectangle(srcX, srcY, srcW, srcH);
                spriteBatch.Draw(texture, destRect, sourceRect, _color);
            }
        }
    }
}