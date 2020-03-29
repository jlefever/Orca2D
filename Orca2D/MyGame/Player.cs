using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orca2D.MyGame
{
    public class Player
    {
        private Vector2 _position = new Vector2(0, 0);

        private const float Speed = 0.1f;

        public void Update(GameTime gameTime, Input input)
        {
            if (input.HasFlag(Input.MoveLeft))
            {
                _position.X -= Speed * gameTime.ElapsedGameTime.Milliseconds;
            }

            if (input.HasFlag(Input.MoveRight))
            {
                _position.X += Speed * gameTime.ElapsedGameTime.Milliseconds;
            }

            if (input.HasFlag(Input.MoveUp))
            {
                _position.Y -= Speed * gameTime.ElapsedGameTime.Milliseconds;
            }

            if (input.HasFlag(Input.MoveDown))
            {
                _position.Y += Speed * gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        public void Draw(SpriteBatch batch, Texture2D texture)
        {
            batch.Draw(texture, _position, Color.White);
        }
    }
}
