using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tile_Engine
{

    /// <summary>
    /// Defini la taille d'un tile
    /// </summary>
    static class Tile
    {
        static public Texture2D TileSetTexture;
        static public int TileWidth = 16;
        static public int TileHeight = 16;

        static public Rectangle GetSourceRectangle(int tileIndex)
        {
            int tileY = tileIndex / (TileSetTexture.Width / TileWidth);
            int tileX = tileIndex % (TileSetTexture.Width / TileWidth);

            return new Rectangle(tileX * TileWidth, tileY * TileHeight, TileWidth, TileHeight);
        }
    }
}
