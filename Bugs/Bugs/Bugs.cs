using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Bugs
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D spriteTexture;
        private int worldWidth = 2048;
        private int worldHeight = 2048;
        private Camera _camera;
        private Texture2D dummyTexture;
        private MouseState old_mouse;
        private float mouseMoveScalingFactor = -1f;
        private float mouseZoomScalingFactor = 1200.0f;

        public Game1()
        {
            // Instance the super-helpful graphics manager  
            graphics = new GraphicsDeviceManager(this);

            // Set vertical trace with the back buffer  
            graphics.SynchronizeWithVerticalRetrace = false;

            // Use multi-sampling to smooth corners of objects  
            graphics.PreferMultiSampling = true;

            // Set the update to run as fast as it can go or  
            // with a target elapsed time between updates  
            IsFixedTimeStep = false;

            // Make the mouse appear  
            IsMouseVisible = true;

            // Set back buffer resolution  
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            // Make full screen  
            //graphics.ToggleFullScreen();

            // Assign content project subfolder  
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
            _camera = new Camera(GraphicsDevice.Viewport);
            _camera.Limits = new Rectangle(0, 0, worldWidth, worldHeight);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteTexture = Content.Load<Texture2D>("dirt");

            dummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Escape)) this.Exit();

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                _camera.Position += new Vector2((Mouse.GetState().X - old_mouse.X) * mouseMoveScalingFactor / _camera.Zoom, (Mouse.GetState().Y - old_mouse.Y) * mouseMoveScalingFactor / _camera.Zoom);
            }
 
            if (Mouse.GetState().ScrollWheelValue != old_mouse.ScrollWheelValue)
            {
                _camera.Zoom += (Mouse.GetState().ScrollWheelValue - old_mouse.ScrollWheelValue) / mouseZoomScalingFactor;
            }

            if (keyboard.IsKeyDown(Keys.R))
            {
                ResetCamera();
            }
 
            old_mouse = Mouse.GetState();

            base.Update(gameTime);
        }

        private void ResetCamera()
        {
            _camera.Zoom = 1f;
            _camera.Position = Vector2.Zero;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        SamplerState.LinearWrap,
                        DepthStencilState.Default,
                        RasterizerState.CullNone,
                        null,
                        _camera.ViewMatrix);

            spriteBatch.Draw(spriteTexture, Vector2.Zero, new Rectangle(0, 0, worldWidth, worldHeight), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
