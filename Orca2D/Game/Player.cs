using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Orca2D.Game
{
    /// <summary>
    /// Our fearless adventurer!
    /// </summary>
    public class Player
    {
        // Animations
        private Animation _idleAnimation;
        private Animation _runAnimation;
        private Animation _jumpAnimation;
        private Animation _celebrateAnimation;
        private Animation _dieAnimation;
        private SpriteEffects _flip = SpriteEffects.None;
        private AnimationPlayer _sprite;

        // Sounds
        private SoundEffect _killedSound;
        private SoundEffect _jumpSound;
        private SoundEffect _fallSound;

        public Level Level { get; }

        public bool IsAlive { get; private set; }

        // Physics state
        public Vector2 Position { get; set; }

        private float _previousBottom;

        public Vector2 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        private Vector2 _velocity;

        // Constants for controlling horizontal movement
        private const float MoveAcceleration = 13000.0f;
        private const float MaxMoveSpeed = 1750.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -3500.0f;
        private const float GravityAcceleration = 3400.0f;
        private const float MaxFallSpeed = 550.0f;
        private const float JumpControlPower = 0.14f;

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround { get; private set; }

        /// <summary>
        /// Current user movement input.
        /// </summary>
        private float _movement;

        // Jumping state
        private bool _isJumping;
        private bool _wasJumping;
        private float _jumpTime;

        private Rectangle _localBounds;

        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                var left = (int)Math.Round(Position.X - _sprite.Origin.X) + _localBounds.X;
                var top = (int)Math.Round(Position.Y - _sprite.Origin.Y) + _localBounds.Y;

                return new Rectangle(left, top, _localBounds.Width, _localBounds.Height);
            }
        }

        /// <summary>
        /// Constructors a new player.
        /// </summary>
        public Player(Level level, Vector2 position)
        {
            Level = level;

            LoadContent();

            Reset(position);
        }

        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public void LoadContent()
        {
            // Load animated textures.
            _idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.1f, true);
            _runAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Run"), 0.1f, true);
            _jumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Jump"), 0.1f, false);
            _celebrateAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Celebrate"), 0.1f, false);
            _dieAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Die"), 0.1f, false);

            // Calculate bounds within texture size.            
            var width = (int)(_idleAnimation.FrameWidth * 0.4);
            var left = (_idleAnimation.FrameWidth - width) / 2;
            var height = (int)(_idleAnimation.FrameWidth * 0.8);
            var top = _idleAnimation.FrameHeight - height;
            _localBounds = new Rectangle(left, top, width, height);

            // Load sounds.            
            _killedSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
            _jumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
            _fallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");
        }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            IsAlive = true;
            _sprite.PlayAnimation(_idleAnimation);
        }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        /// <remarks>
        /// We pass in all of the input states so that our game is only polling the hardware
        /// once per frame. We also pass the game's orientation because when using the accelerometer,
        /// we need to reverse our motion when the orientation is in the LandscapeRight orientation.
        /// </remarks>
        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            GetInput(keyboardState);

            ApplyPhysics(gameTime);

            if (IsAlive && IsOnGround)
            {
                _sprite.PlayAnimation(Math.Abs(Velocity.X) - 0.02f > 0 ? _runAnimation : _idleAnimation);
            }

            // Clear input.
            _movement = 0.0f;
            _isJumping = false;
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void GetInput(KeyboardState keyboardState)
        {
            // Ignore small movements to prevent running in place.
            if (Math.Abs(_movement) < 0.5f)
            {
                _movement = 0.0f;
            }

            // If any digital horizontal movement input is found, override the analog movement.
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                _movement = -1.0f;
            }
            else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                _movement = 1.0f;
            }

            // Check if the player wants to jump.
            _isJumping =
                keyboardState.IsKeyDown(Keys.Space) ||
                keyboardState.IsKeyDown(Keys.Up) ||
                keyboardState.IsKeyDown(Keys.W);
        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            _velocity.X += _movement * MoveAcceleration * elapsed;
            _velocity.Y = MathHelper.Clamp(_velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            _velocity.Y = DoJump(_velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (IsOnGround)
                _velocity.X *= GroundDragFactor;
            else
                _velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.
            _velocity.X = MathHelper.Clamp(_velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            Position += _velocity * elapsed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // If the player is now colliding with the level, separate them.
            HandleCollisions();

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
            {
                _velocity.X = 0;
            }

            if (Position.Y == previousPosition.Y)
            {
                _velocity.Y = 0;
            }
        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (_isJumping)
            {
                // Begin or continue a jump
                if ((!_wasJumping && IsOnGround) || _jumpTime > 0.0f)
                {
                    if (_jumpTime == 0.0f)
                        _jumpSound.Play();

                    _jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _sprite.PlayAnimation(_jumpAnimation);
                }

                // If we are in the ascent of the jump
                if (0.0f < _jumpTime && _jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(_jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    _jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                _jumpTime = 0.0f;
            }
            _wasJumping = _isJumping;

            return velocityY;
        }

        /// <summary>
        /// Detects and resolves all collisions between the player and his neighboring
        /// tiles. When a collision is detected, the player is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </summary>
        private void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            var bounds = BoundingRectangle;
            var leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            var rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            var topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            var bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // Reset flag to search for ground collision.
            IsOnGround = false;

            // For each potentially colliding tile,
            for (var y = topTile; y <= bottomTile; ++y)
            {
                for (var x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    var collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        var tileBounds = Level.GetBounds(x, y);
                        var depth = bounds.GetIntersectionDepth(tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            var absDepthX = Math.Abs(depth.X);
                            var absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (_previousBottom <= tileBounds.Top)
                                    IsOnGround = true;

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            _previousBottom = bounds.Bottom;
        }

        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).
        /// </param>
        public void OnKilled(Enemy killedBy)
        {
            IsAlive = false;

            if (killedBy != null)
            {
                _killedSound.Play();
            }
            else
            {
                _fallSound.Play();
            }

            _sprite.PlayAnimation(_dieAnimation);
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        public void OnReachedExit()
        {
            _sprite.PlayAnimation(_celebrateAnimation);
        }

        /// <summary>
        /// Draws the animated player.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Flip the sprite to face the way we are moving.
            if (Velocity.X > 0)
            {
                _flip = SpriteEffects.FlipHorizontally;
            }
            else if (Velocity.X < 0)
            {
                _flip = SpriteEffects.None;
            }

            // Draw that sprite.
            _sprite.Draw(gameTime, spriteBatch, Position, _flip);
        }
    }
}
