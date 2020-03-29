using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Orca2D.MyGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MyGame : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly IInputProvider _inputProvider;
        private SpriteBatch _spriteBatch;
        private Stage _stage;
        private Player _player;
        private Texture2D _playerTexture;

        public MyGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _inputProvider = new KeyboardInputProvider();
            _player = new Player();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var texturePack = new TexturePack
            {
                Exit = Content.Load<Texture2D>("Tiles/Exit"),
                PlatformBlock = Content.Load<Texture2D>("Tiles/Platform"),
                PassableBlock = Content.Load<Texture2D>("Tiles/BlockB0"),
                ImpassableBlock = Content.Load<Texture2D>("Tiles/BlockA0"),
                MonsterA = Content.Load<Texture2D>("Tiles/BlockA1"),
                MonsterB = Content.Load<Texture2D>("Tiles/BlockA2"),
                MonsterC = Content.Load<Texture2D>("Tiles/BlockA3"),
                MonsterD = Content.Load<Texture2D>("Tiles/BlockA4"),
                Gem = Content.Load<Texture2D>("Sprites/Gem")
            };

            _playerTexture = Content.Load<Texture2D>("Sprites/Gem");

            var parser = new StageFileParser();

            using (var stream = TitleContainer.OpenStream("Content/Levels/1.txt"))
            {
                var reader = new StreamReader(stream);
                var text = reader.ReadToEnd();
                var tiles = parser.Parse(text);
                _stage = new Stage(tiles, texturePack, new Vector2(40, 32));
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _player.Update(gameTime, _inputProvider.PollInput());

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _stage.Draw(_spriteBatch);
            _player.Draw(_spriteBatch, _playerTexture);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
