using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orca2D.MyGame
{
    public class TexturePack
    {
        public Texture2D Air { get; set; }
        public Texture2D Start { get; set; }
        public Texture2D Exit { get; set; }
        public Texture2D PlatformBlock { get; set; }
        public Texture2D PassableBlock { get; set; }
        public Texture2D ImpassableBlock { get; set; }
        public Texture2D MonsterA { get; set; }
        public Texture2D MonsterB { get; set; }
        public Texture2D MonsterC { get; set; }
        public Texture2D MonsterD { get; set; }
        public Texture2D Gem { get; set; }

        public Texture2D GetTexture2D(TileKind tile)
        {
            switch (tile)
            {
                case TileKind.Air:
                    return Air;
                case TileKind.Start:
                    return Start;
                case TileKind.Exit:
                    return Exit;
                case TileKind.PlatformBlock:
                    return PlatformBlock;
                case TileKind.PassableBlock:
                    return PassableBlock;
                case TileKind.ImpassableBlock:
                    return ImpassableBlock;
                case TileKind.MonsterA:
                    return MonsterA;
                case TileKind.MonsterB:
                    return MonsterB;
                case TileKind.MonsterC:
                    return MonsterC;
                case TileKind.MonsterD:
                    return MonsterD;
                case TileKind.Gem:
                    return Gem;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tile), tile, null);
            }
        }
    }
}
