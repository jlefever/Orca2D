using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orca2D.Extensions;

namespace Orca2D.MyGame
{
    public class Stage
    {
        private readonly TileKind[,] _tiles;
        private readonly TexturePack _texturePack;
        private readonly Vector2 _tileSize;

        public Stage(TileKind[,] tiles, TexturePack texturePack, Vector2 tileSize)
        {
            _tiles = tiles;
            _texturePack = texturePack;
            _tileSize = tileSize;
        }

        public void Draw(SpriteBatch batch)
        {
            for (var x = 0; x < _tiles.GetWidth(); x++)
            {
                for (var y = 0; y < _tiles.GetHeight(); y++)
                {
                    var texture = _texturePack.GetTexture2D(_tiles[x, y]);

                    if (texture == null)
                    {
                        continue;
                    }

                    // Why are these switched?
                    var position = new Vector2(y, x) * _tileSize;
                    batch.Draw(texture, position, Color.White);
                }
            }
        }
    }
}
