using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Orca2D.MyGame.Entities;
using System.Collections.Generic;

namespace Orca2D.MyGame
{
    public class SpritePackLoader
    {
        private readonly ContentManager _manager;
        private readonly IDictionary<string, Texture2D> _textures;

        public SpritePackLoader(ContentManager manager)
        {
            _manager = manager;
            _textures = new Dictionary<string, Texture2D>();
        }

        public SpritePack Load(IEnumerable<SpriteSheetDto> sheets)
        {
            var pack = new SpritePack();

            foreach (var sheet in sheets)
            {
                if (!_textures.TryGetValue(sheet.TextureName, out var texture))
                {
                    texture = _manager.Load<Texture2D>(sheet.TextureName);
                    _textures.Add(sheet.TextureName, texture);
                }

                foreach (var sprite in sheet.Sprites)
                {
                    SetSprite(pack, sprite.Key, ToSprite(sprite.Value, texture));
                }
            }

            return pack;
        }

        private static Sprite ToSprite(SpriteDto dto, Texture2D texture)
        {
            return new Sprite(texture, new Rectangle(dto.X, dto.Y, dto.Width, dto.Height));
        }

        private static void SetSprite(SpritePack pack, string name, Sprite value)
        {
            switch (name)
            {
                case "King1":
                    pack.King1 = value;
                    break;
                case "King2":
                    pack.King2 = value;
                    break;
                case "King3":
                    pack.King3 = value;
                    break;
                case "King4":
                    pack.King4 = value;
                    break;
                case "King5":
                    pack.King5 = value;
                    break;
                case "King6":
                    pack.King6 = value;
                    break;
                case "King7":
                    pack.King7 = value;
                    break;
                case "King8":
                    pack.King8 = value;
                    break;
            }
        }
    }
}
