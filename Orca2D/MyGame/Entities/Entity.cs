using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orca2D.MyGame.Entities
{
    public interface IEntity
    {
        Sprite Sprite { get; }
        Vector2 Position { get; }
        Vector2 CollisionBox { get; }
        CollisionType CollisionType { get; }
    }

    public enum PlayerState
    {
        Idle,
        Walking,
    }

    public class PlayerEntity
    {
        private readonly Sprite _idleSprite;
        private readonly AnimatedSprite _walkingSprite;
        private Vector2 _position;
        private readonly float _speed;

        public PlayerEntity(Sprite idleSprite, AnimatedSprite walkingSprite, Vector2 position, float speed)
        {
            _idleSprite = idleSprite;
            _walkingSprite = walkingSprite;
            _position = position;
            _speed = speed;
        }

        //public Vector2 CollisionBox { get; }
        //public CollisionType CollisionType { get; }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(_walkingSprite.CurrentFrame.Texture, _position, _walkingSprite.CurrentFrame.Source, Color.White);
        }

        public void Update(GameTime gameTime, Input input)
        {
            _walkingSprite.Update(gameTime.ElapsedGameTime.Milliseconds);

            if (input.HasFlag(Input.MoveLeft))
            {
                _position.X -= _speed * gameTime.ElapsedGameTime.Milliseconds;
            }

            if (input.HasFlag(Input.MoveRight))
            {
                _position.X += _speed * gameTime.ElapsedGameTime.Milliseconds;
            }

            if (input.HasFlag(Input.MoveUp))
            {
                _position.Y -= _speed * gameTime.ElapsedGameTime.Milliseconds;
            }

            if (input.HasFlag(Input.MoveDown))
            {
                _position.Y += _speed * gameTime.ElapsedGameTime.Milliseconds;
            }
        }
    }
}
