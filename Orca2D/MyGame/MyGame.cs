using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Lifetime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Orca2D.MyGame.Entities;

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
        private SpritePack _spritePack;
        private Stage _stage;
        private PlayerEntity _player;
        

        public MyGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _inputProvider = new KeyboardInputProvider();
            _spritePack = new SpritePack();
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

            // Load all sprites
            var mapJson = File.ReadAllText("Content/External/sprite_maps.json");
            var sheets = JsonConvert.DeserializeObject<SpriteSheetDto[]>(mapJson);
            _spritePack = new SpritePackLoader(Content).Load(sheets);

            // Create walking sprite
            var animation = new AnimatedSprite(new []
            {
                _spritePack.King1,
                _spritePack.King2,
                _spritePack.King3,
                _spritePack.King4
            }, 300);

            // Create Player
            _player = new PlayerEntity(_spritePack.King1, animation, Vector2.One, 0.5f);
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
            _player.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
