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
    public class Bugs : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BugObject bug1;
        SpriteFont spriteFont;
        Vector2 spriteFontPosition;

        public Bugs()
        {
            graphics = new GraphicsDeviceManager(this);
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
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D bugTexture = Content.Load<Texture2D>("bug");
            bug1 = new BugObject(bugTexture, new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2), new Vector2(0f, -2f));

            spriteFont = Content.Load<SpriteFont>("SpriteFont1");
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

            bug1.Position += bug1.Velocity;

            CheckWallCollision();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            bug1.Draw(spriteBatch);

            string output = string.Format("{0:0}, {1:0}", bug1.Position.X, bug1.Position.Y);
            Vector2 fontOrigin = spriteFont.MeasureString(output) / 2;
            spriteFontPosition.X = fontOrigin.X;
            spriteFontPosition.Y = fontOrigin.Y;
            spriteBatch.DrawString(spriteFont, output, spriteFontPosition, Color.Black, 0, fontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void CheckWallCollision()
        {
            Viewport viewport = GraphicsDevice.Viewport;
            if (bug1.Position.Y < bug1.BoundingBox.Height / 2)
            {
                bug1.Velocity.Y = 0;
            }
        }
    }
}
