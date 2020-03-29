namespace Orca2D.MyGame
{
    public class StageFileParser
    {
        public TileKind[,] Parse(string text)
        {
            var width = GetWidth(text);
            var height = GetHeight(text, width);

            var grid = new TileKind[height, width];
            var currentTile = 0;

            foreach (var c in text)
            {
                if (c == '\n' || c == '\r')
                {
                    continue;
                }

                grid[currentTile / width, currentTile % width] = ParseTile(c);
                currentTile += 1;
            }

            return grid;
        }

        private static TileKind ParseTile(char c)
        {
            switch (c)
            {
                case '.':
                    return TileKind.Air;
                case 'X':
                    return TileKind.Exit;
                case '1':
                    return TileKind.Start;
                case 'G':
                    return TileKind.Gem;
                case '-':
                case '~':
                    return TileKind.PlatformBlock;
                case ':':
                    return TileKind.PassableBlock;
                case '#':
                    return TileKind.ImpassableBlock;
                case 'A':
                    return TileKind.MonsterA;
                case 'B':
                    return TileKind.MonsterB;
                case 'C':
                    return TileKind.MonsterC;
                case 'D':
                    return TileKind.MonsterD;
                default:
                    throw new BadLevelFileException();
            }
        }

        private static int GetWidth(string text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    return i - 1;
                }
            }

            return text.Length;
        }

        private static int GetHeight(string text, int width)
        {
            var length = 0;

            foreach (var c in text)
            {
                if (c == '\n' || c == '\r')
                {
                    continue;
                }

                length += 1;
            }

            return length / width;
        }
    }
}
