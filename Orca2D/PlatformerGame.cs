using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Orca2D.Game;

namespace Orca2D
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PlatformerGame : Microsoft.Xna.Framework.Game
    {
        // Resources for drawing
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly Vector2 _baseScreenSize = new Vector2(800, 480);
        private Matrix _globalTransformation;
        private int _backBufferWidth, _backBufferHeight;

        // Global content
        private SpriteFont _hudFont;

        private Texture2D _winOverlay;
        private Texture2D _loseOverlay;
        private Texture2D _diedOverlay;

        // Meta-level game state
        private int _levelIndex = -1;
        private Level _level;
        private bool _wasContinuePressed;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

        // We store our input states so that we only poll once per frame, 
        // then we use the same input state wherever needed
        private KeyboardState _keyboardState;

        // The number of levels in the Levels directory of our content. We assume that
        // levels in our content are 0-based and that all numbers under this constant
        // have a level file present. This allows us to not need to check for the file
        // or handle exceptions, both of which can add unnecessary time to level loading.
        private const int NumberOfLevels = 3;

        public PlatformerGame()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false
            };

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load fonts
            _hudFont = Content.Load<SpriteFont>("Fonts/Hud");

            // Load overlay textures
            _winOverlay = Content.Load<Texture2D>("Overlays/you_win");
            _loseOverlay = Content.Load<Texture2D>("Overlays/you_lose");
            _diedOverlay = Content.Load<Texture2D>("Overlays/you_died");

            ScalePresentationArea();

            //Known issue that you get exceptions if you use Media PLayer while connected to your PC
            //See http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66
            //Which means its impossible to test this from VS.
            //So we have to catch the exception and throw it away
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Content.Load<Song>("Sounds/Music"));
            }
            catch (Exception)
            {
                // ignored
            }

            LoadNextLevel();
        }

        public void ScalePresentationArea()
        {
            //Work out how much we need to scale our graphics to fill the screen
            _backBufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            _backBufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            var horScaling = _backBufferWidth / _baseScreenSize.X;
            var verScaling = _backBufferHeight / _baseScreenSize.Y;
            var screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            _globalTransformation = Matrix.CreateScale(screenScalingFactor);
            System.Diagnostics.Debug.WriteLine("Screen Size - Width[" + GraphicsDevice.PresentationParameters.BackBufferWidth + "] Height [" + GraphicsDevice.PresentationParameters.BackBufferHeight + "]");
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Confirm the screen has not been resized by the user
            if (_backBufferHeight != GraphicsDevice.PresentationParameters.BackBufferHeight ||
                _backBufferWidth != GraphicsDevice.PresentationParameters.BackBufferWidth)
            {
                ScalePresentationArea();
            }

            // Handle polling for our input and handling high-level input
            HandleInput(gameTime);

            // Update our level, passing down the GameTime along with all of our input states
            _level.Update(gameTime, _keyboardState);

            base.Update(gameTime);
        }

        private void HandleInput(GameTime gameTime)
        {
            _keyboardState = Keyboard.GetState();

            var continuePressed = _keyboardState.IsKeyDown(Keys.Space);

            // Perform the appropriate action to advance the game and
            // to get the player back to playing.
            if (!_wasContinuePressed && continuePressed)
            {
                if (!_level.Player.IsAlive)
                {
                    _level.StartNewLife();
                }
                else if (_level.TimeRemaining == TimeSpan.Zero)
                {
                    if (_level.ReachedExit)
                    {
                        LoadNextLevel();
                    }
                    else
                    {
                        ReloadCurrentLevel();
                    }
                }
            }

            _wasContinuePressed = continuePressed;
        }

        private void LoadNextLevel()
        {
            // move to the next level
            _levelIndex = (_levelIndex + 1) % NumberOfLevels;

            // Unloads the content for the current level before loading the next one
            _level?.Dispose();

            // Load the level
            var levelPath = $"Content/Levels/{_levelIndex}.txt";

            using (var fileStream = TitleContainer.OpenStream(levelPath))
            {
                _level = new Level(Services, fileStream, _levelIndex);
            }
        }

        private void ReloadCurrentLevel()
        {
            --_levelIndex;
            LoadNextLevel();
        }

        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, _globalTransformation);

            _level.Draw(gameTime, _spriteBatch);

            DrawHud();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawHud()
        {
            var (x, y, _, _) = GraphicsDevice.Viewport.TitleSafeArea;
            var hudLocation = new Vector2(x, y);

            var center = new Vector2(_baseScreenSize.X / 2, _baseScreenSize.Y / 2);

            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            var timeString = "TIME: " + _level.TimeRemaining.Minutes.ToString("00") + ":" + _level.TimeRemaining.Seconds.ToString("00");
            Color timeColor;
            if (_level.TimeRemaining > WarningTime ||
                _level.ReachedExit ||
                (int)_level.TimeRemaining.TotalSeconds % 2 == 0)
            {
                timeColor = Color.Yellow;
            }
            else
            {
                timeColor = Color.Red;
            }

            DrawShadowedString(_hudFont, timeString, hudLocation, timeColor);

            // Draw score
            var timeHeight = _hudFont.MeasureString(timeString).Y;
            DrawShadowedString(_hudFont, "SCORE: " + _level.Score.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f), Color.Yellow);

            // Determine the status overlay message to show.
            Texture2D status = null;
            if (_level.TimeRemaining == TimeSpan.Zero)
            {
                status = _level.ReachedExit ? _winOverlay : _loseOverlay;
            }
            else if (!_level.Player.IsAlive)
            {
                status = _diedOverlay;
            }

            if (status == null)
            {
                return;
            }

            // Draw status message.
            var statusSize = new Vector2(status.Width, status.Height);
            _spriteBatch.Draw(status, center - statusSize / 2, Color.White);
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            _spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            _spriteBatch.DrawString(font, value, position, color);
        }
    }
}
