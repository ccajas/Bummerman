
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Lidgren.Network;
using Meteor.ECS;

namespace Bummerman
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class BummermanGame : Game
    {
        GraphicsDeviceManager graphics;

        /// Screens to update and set
        ScreenElement currentScreen, nextScreen;

        /// Diagnostic tool
        Stopwatch stopWatch = new Stopwatch();
        Stopwatch stopWatch2 = new Stopwatch(); 

        /// <summary>
        /// Game constructor
        /// </summary>
        public BummermanGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Graphics settings
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Add window handler
            Exiting += delegate(Object o, EventArgs e)
            {
                if (currentScreen != null)
                    currentScreen.UnloadContent();
            };

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Launch the first screen.
            currentScreen = new ScreenElements.GameSelectionScreen(this, null);
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
            // Reset the timer
            stopWatch.Restart();

            // Allows the game to exit
            if (currentScreen == null)
            {
                this.Exit();
            }
            else
            {
                // Update screens
                nextScreen = currentScreen.Update(gameTime.ElapsedGameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Draw the current screen
            (currentScreen as DrawableScreenElement).Draw(gameTime.ElapsedGameTime);

            // Swap screen for the next frame
            if (nextScreen != currentScreen)
                currentScreen = nextScreen;

            base.Draw(gameTime);
        }
    }
}
