using System;

namespace Orca2D.MyGame
{
    public class AnimatedSprite
    {
        private readonly Sprite[] _frames;
        private readonly int _frameRate;

        private int _currentFrame;
        private int _currentTime;

        public AnimatedSprite(Sprite[] frames, int frameRate)
        {
            _frames = frames;
            _frameRate = frameRate;
            _currentFrame = 0;
            _currentTime = 0;
        }

        public Sprite CurrentFrame => _frames[_currentFrame];

        public void Update(int elapsed)
        {
            _currentTime += elapsed;
            var offset = (int)Math.Floor(_currentTime / 100.0 * _frameRate);
            _currentFrame += offset;
            _currentFrame %= _frames.Length;
        }

        public void Reset()
        {
            _currentFrame = 0;
        }
    }
}
