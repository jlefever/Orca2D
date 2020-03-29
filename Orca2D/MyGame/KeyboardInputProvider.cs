using Microsoft.Xna.Framework.Input;

namespace Orca2D.MyGame
{
    public class KeyboardInputProvider : IInputProvider
    {
        public Input PollInput()
        {
            var input = Input.None;

            if (IsPressed(Keys.Left))
            {
                input |= Input.MoveLeft;
            }

            if (IsPressed(Keys.Right))
            {
                input |= Input.MoveRight;
            }

            if (IsPressed(Keys.Up))
            {
                input |= Input.MoveUp;
            }

            if (IsPressed(Keys.Down))
            {
                input |= Input.MoveDown;
            }

            if (IsPressed(Keys.Space))
            {
                input |= Input.Jump;
            }

            return input;
        }

        private static bool IsPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }
    }
}
