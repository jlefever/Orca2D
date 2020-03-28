using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orca2D.Game
{
    /// <summary>
    /// A monster who is impeding the progress of our fearless adventurer.
    /// </summary>
    public class Enemy
    {
        public Level Level { get; }

        /// <summary>
        /// Position in world space of the bottom center of this enemy.
        /// </summary>
        public Vector2 Position { get; private set; }

        private Rectangle _localBounds;

        /// <summary>
        /// Gets a rectangle which bounds this enemy in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                var left = (int)Math.Round(Position.X - _animationPlayer.Origin.X) + _localBounds.X;
                var top = (int)Math.Round(Position.Y - _animationPlayer.Origin.Y) + _localBounds.Y;

                return new Rectangle(left, top, _localBounds.Width, _localBounds.Height);
            }
        }

        // Animations
        private Animation _runAnimation;
        private Animation _idleAnimation;
        private AnimationPlayer _animationPlayer;

        /// <summary>
        /// The direction this enemy is facing and moving along the X axis.
        /// </summary>
        private FaceDirection _direction = FaceDirection.Left;

        /// <summary>
        /// How long this enemy has been waiting before turning around.
        /// </summary>
        private float _waitTime;

        /// <summary>
        /// How long to wait before turning around.
        /// </summary>
        private const float MaxWaitTime = 0.5f;

        /// <summary>
        /// The speed at which this enemy moves along the X axis.
        /// </summary>
        private const float MoveSpeed = 64.0f;

        /// <summary>
        /// Constructs a new Enemy.
        /// </summary>
        public Enemy(Level level, Vector2 position, string spriteSet)
        {
            Level = level;
            Position = position;

            LoadContent(spriteSet);
        }

        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
        /// </summary>
        public void LoadContent(string spriteSet)
        {
            // Load animations.
            spriteSet = "Sprites/" + spriteSet + "/";
            _runAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Run"), 0.1f, true);
            _idleAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Idle"), 0.15f, true);
            _animationPlayer.PlayAnimation(_idleAnimation);

            // Calculate bounds within texture size.
            var width = (int)(_idleAnimation.FrameWidth * 0.35);
            var left = (_idleAnimation.FrameWidth - width) / 2;
            var height = (int)(_idleAnimation.FrameWidth * 0.7);
            var top = _idleAnimation.FrameHeight - height;
            _localBounds = new Rectangle(left, top, width, height);
        }


        /// <summary>
        /// Paces back and forth along a platform, waiting at either end.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate tile position based on the side we are walking towards.
            var posX = Position.X + _localBounds.Width / 2 * (int)_direction;
            var tileX = (int)Math.Floor(posX / Tile.Width) - (int)_direction;
            var tileY = (int)Math.Floor(Position.Y / Tile.Height);

            if (_waitTime > 0)
            {
                // Wait for some amount of time.
                _waitTime = Math.Max(0.0f, _waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (_waitTime <= 0.0f)
                {
                    // Then turn around.
                    _direction = (FaceDirection)(-(int)_direction);
                }
            }
            else
            {
                // If we are about to run into a wall or off a cliff, start waiting.
                if (Level.GetCollision(tileX + (int)_direction, tileY - 1) == TileCollision.Impassable ||
                    Level.GetCollision(tileX + (int)_direction, tileY) == TileCollision.Passable)
                {
                    _waitTime = MaxWaitTime;
                }
                else
                {
                    // Move in the current direction.
                    var velocity = new Vector2((int)_direction * MoveSpeed * elapsed, 0.0f);
                    Position = Position + velocity;
                }
            }
        }

        /// <summary>
        /// Draws the animated enemy.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Stop running when the game is paused or before turning around.
            if (!Level.Player.IsAlive ||
                Level.ReachedExit ||
                Level.TimeRemaining == TimeSpan.Zero ||
                _waitTime > 0)
            {
                _animationPlayer.PlayAnimation(_idleAnimation);
            }
            else
            {
                _animationPlayer.PlayAnimation(_runAnimation);
            }


            // Draw facing the way the enemy is moving.
            var flip = _direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            _animationPlayer.Draw(gameTime, spriteBatch, Position, flip);
        }
    }
}
