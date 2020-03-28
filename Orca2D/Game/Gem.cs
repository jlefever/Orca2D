using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Orca2D.Game
{
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    public class Gem
    {
        private Texture2D _texture;
        private Vector2 _origin;
        private SoundEffect _collectedSound;

        public const int PointValue = 30;
        public readonly Color Color = Color.Yellow;

        // The gem is animated from a base position along the Y axis.
        private readonly Vector2 _basePosition;
        private float _bounce;

        public Level Level { get; }

        /// <summary>
        /// Gets the current position of this gem in world space.
        /// </summary>
        public Vector2 Position => _basePosition + new Vector2(0.0f, _bounce);

        /// <summary>
        /// Gets a circle which bounds this gem in world space.
        /// </summary>
        public Circle BoundingCircle => new Circle(Position, Tile.Width / 3.0f);

        /// <summary>
        /// Constructs a new gem.
        /// </summary>
        public Gem(Level level, Vector2 position)
        {
            Level = level;
            _basePosition = position;

            LoadContent();
        }

        /// <summary>
        /// Loads the gem texture and collected sound.
        /// </summary>
        public void LoadContent()
        {
            _texture = Level.Content.Load<Texture2D>("Sprites/Gem");
            _origin = new Vector2(_texture.Width / 2.0f, _texture.Height / 2.0f);
            _collectedSound = Level.Content.Load<SoundEffect>("Sounds/GemCollected");
        }

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Bounce control constants
            const float bounceHeight = 0.18f;
            const float bounceRate = 3.0f;
            const float bounceSync = -0.75f;

            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring gems bounce in a nice wave pattern.            
            var t = gameTime.TotalGameTime.TotalSeconds * bounceRate + Position.X * bounceSync;
            _bounce = (float)Math.Sin(t) * bounceHeight * _texture.Height;
        }

        /// <summary>
        /// Called when this gem has been collected by a player and removed from the level.
        /// </summary>
        /// <param name="collectedBy">
        /// The player who collected this gem. Although currently not used, this parameter would be
        /// useful for creating special powerup gems. For example, a gem could make the player invincible.
        /// </param>
        public void OnCollected(Player collectedBy)
        {
            _collectedSound.Play();
        }

        /// <summary>
        /// Draws a gem in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Color, 0.0f, _origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
