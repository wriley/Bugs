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
using System.Threading;

namespace Bugs
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Vector2 spriteFontPosition;
        private Texture2D backgroundTexture;
        private SpriteFont spriteFont;
        private int _worldWidth = 1024;
        private int _worldHeight = 1024;
        private Rectangle _worldLimits;
        private Camera _camera;
        private MouseState old_mouse;
        private float mouseMoveScalingFactor = -1f;
        private float mouseZoomScalingFactor = 1200.0f;
        private float frameRate;
        private float updateRate;
        private string output = "";

        private int maxBugs = 500;
        private List<BugObject> bugsList;
        private Texture2D bugTexture;

        private Random _random = new Random();

        public Game1()
        {
            // Instance the super-helpful graphics manager  
            graphics = new GraphicsDeviceManager(this);

            // Set vertical trace with the back buffer  
            graphics.SynchronizeWithVerticalRetrace = true;

            // Use multi-sampling to smooth corners of objects  
            graphics.PreferMultiSampling = true;

            // Set the update to run as fast as it can go or  
            // with a target elapsed time between updates  
            IsFixedTimeStep = true;

            // Make the mouse appear  
            IsMouseVisible = true;

            // Set back buffer resolution  
            graphics.PreferredBackBufferWidth = 600;
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
            _camera.Limits = new Rectangle(0, 0, _worldWidth, _worldHeight);
            bugsList = new List<BugObject>();
            _worldLimits = new Rectangle(0, 0, _worldWidth, _worldHeight);
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
            spriteFont = Content.Load<SpriteFont>("CourierNew");
            backgroundTexture = Content.Load<Texture2D>("dirt_plain");
            bugTexture = Content.Load<Texture2D>("bug");
            Vector2 startPosition = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);

            for (int i = 0; i < maxBugs; i++)
            {
                float rot = 6.28318531f * (float)_random.NextDouble();
                bugsList.Add(new BugObject(bugTexture, startPosition, rot));
                Thread.Sleep(1);
            }
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
            updateRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (keyboard.IsKeyDown(Keys.Escape)) this.Exit();

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                _camera.Position += new Vector2((mouse.X - old_mouse.X) * mouseMoveScalingFactor / _camera.Zoom, (mouse.Y - old_mouse.Y) * mouseMoveScalingFactor / _camera.Zoom);
            }

            if (mouse.ScrollWheelValue != old_mouse.ScrollWheelValue)
            {
                _camera.Zoom += (mouse.ScrollWheelValue - old_mouse.ScrollWheelValue) / mouseZoomScalingFactor;
            }

            if (keyboard.IsKeyDown(Keys.R))
            {
                ResetCamera();
            }

            if (mouse.RightButton == ButtonState.Pressed && old_mouse.RightButton == ButtonState.Released)
            {
                if (bugsList.Count < maxBugs)
                {
                    Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
                    Matrix transform = Matrix.Invert(_camera.ViewMatrix);
                    Vector2.Transform(ref mousePos, ref transform, out mousePos);
                    float rot = 5f * (float)_random.NextDouble() + 1;
                    bugsList.Add(new BugObject(bugTexture, mousePos, rot));
                }
            }

            bugsList.RemoveAll(delegate(BugObject b) { return b.isDead(); });

            foreach (BugObject bug in bugsList)
            {
                bug.Update(gameTime, _worldLimits);
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
            frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        SamplerState.LinearWrap,
                        DepthStencilState.Default,
                        RasterizerState.CullNone,
                        null,
                        _camera.ViewMatrix);

            output = string.Format("FPS: {0:0.0}  UPS: {1:0.000} Count: {2}", frameRate, updateRate, bugsList.Count);
            Vector2 fontOrigin = spriteFont.MeasureString(output) / 2;
            Matrix transform = Matrix.Invert(_camera.ViewMatrix);
            Vector2 viewportPos = new Vector2(graphics.GraphicsDevice.Viewport.X, graphics.GraphicsDevice.Viewport.Y);
            Vector2.Transform(ref viewportPos, ref transform, out spriteFontPosition);
            spriteFontPosition += fontOrigin / _camera.Zoom;
            spriteBatch.DrawString(spriteFont, output, spriteFontPosition, Color.White, 0, fontOrigin, 1.0f / _camera.Zoom, SpriteEffects.None, 0);


            foreach (BugObject bug in bugsList)
            {
                bug.Draw(spriteBatch);
            }

            spriteBatch.Draw(backgroundTexture, Vector2.Zero, new Rectangle(0, 0, _worldWidth, _worldHeight), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
