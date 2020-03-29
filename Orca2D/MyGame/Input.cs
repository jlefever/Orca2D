using System;

namespace Orca2D.MyGame
{
    [Flags]
    public enum Input : short
    {
        None = 0,
        MoveLeft = 1,
        MoveRight = 2,
        MoveUp = 4,
        MoveDown = 8,
        Jump = 16
    }
}
