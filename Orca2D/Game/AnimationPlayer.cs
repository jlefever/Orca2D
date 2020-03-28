using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orca2D.Game
{
    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    public struct AnimationPlayer
    {
        /// <summary>
        /// Gets the animation which is currently playing.
        /// </summary>
        private Animation _animation;

        /// <summary>
        /// Gets the index of the current frame in the animation.
        /// </summary>
        private int _frameIndex;

        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        private float _time;

        /// <summary>
        /// Gets a texture origin at the bottom center of each frame.
        /// </summary>
        public Vector2 Origin => new Vector2(_animation.FrameWidth / 2.0f, _animation.FrameHeight);

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        public void PlayAnimation(Animation ani)
        {
            // If this animation is already running, do not restart it.
            if (_animation == ani)
            {
                return;
            }

            // Start the new animation.
            _animation = ani;
            _frameIndex = 0;
            _time = 0.0f;
        }

        /// <summary>
        /// Advances the time position and draws the current frame of the animation.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (_animation == null)
            {
                throw new NotSupportedException("No animation is currently playing.");
            }

            // Process passing time.
            _time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (_time > _animation.FrameTime)
            {
                _time -= _animation.FrameTime;

                // Advance the frame index; looping or clamping as appropriate.
                if (_animation.IsLooping)
                {
                    _frameIndex = (_frameIndex + 1) % _animation.FrameCount;
                }
                else
                {
                    _frameIndex = Math.Min(_frameIndex + 1, _animation.FrameCount - 1);
                }
            }

            // Calculate the source rectangle of the current frame.
            var source = GetSourceRectangle(_animation.Texture.Height, _frameIndex);

            // Draw the current frame.
            spriteBatch.Draw(_animation.Texture, position, source, Color.White, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
        }

        private static Rectangle GetSourceRectangle(int textureHeight, int frameIndex)
        {
            return new Rectangle(frameIndex * textureHeight, 0, textureHeight, textureHeight);
        }
    }
}
