using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orca2D.MyGame
{
    public class Sprite
    {
        public Sprite(Texture2D texture, Rectangle source)
        {
            Texture = texture;
            Source = source;
        }

        public Texture2D Texture { get; }
        public Rectangle Source { get; }
    }
}