using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Bummerman
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // ECS entity manager
        SystemManager systemManager;

        // Sprite textures and other assets
        Dictionary<string, Texture2D> textureCollection;
        RenderTarget2D screenRT;
        SpriteFont debugFont;
        
        // Virtual resolution for adaptive resizing
        int virtualBufferWidth = 1920;
        int virtualBufferHeight = 1280;

        // Default to virtual res ratio
        float virtualResolutionRatio = 1f;

        // Game resources
        Level level;

        public Game1()
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
            // TODO: Add your initialization logic here
            textureCollection = new Dictionary<string, Texture2D>();
            systemManager = new SystemManager();
            level = new Level();

            // Graphics settings
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();

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

            // load your game content here
            textureCollection.Add("stone", Content.Load<Texture2D>("textures/spritesheet_stone"));
            textureCollection.Add("metal", Content.Load<Texture2D>("textures/spritesheet_metal"));
            textureCollection.Add("player", Content.Load<Texture2D>("textures/player"));
            textureCollection.Add("player1", Content.Load<Texture2D>("textures/bomber1"));
            textureCollection.Add("round_explosion", Content.Load<Texture2D>("textures/round_explosion"));

            // Set render target to virtual resolution
            screenRT = new RenderTarget2D(GraphicsDevice,
                virtualBufferWidth,
                virtualBufferHeight
            );
            debugFont = Content.Load<SpriteFont>("debug");
            virtualResolutionRatio = (float)GraphicsDevice.Viewport.Width / (float)virtualBufferWidth;

            // Create systems and entity templates
            systemManager.SetupSystems(textureCollection);

            // Load level entities
            level.Load(systemManager.Entities);
            
            // Finished loading content

            // Make a 1x1 texture named pixel.  
            pixel = new Texture2D(GraphicsDevice, 1, 1);

            // Set the texture data with our color information.  
            pixel.SetData<Color>(colorData);
        }

        // Background pixel texture 
        private Texture2D pixel;
        private Color[] colorData = { Color.White };  

        /// <summary>
        /// Make a solid color rectangle
        /// </summary>
        public void ColorRectangle(Color color, Rectangle rect, SpriteBatch spriteBatch)
        {
            // Draw a fancy rectangle.  
            spriteBatch.Draw(pixel, rect, color);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            TimeSpan frameStepTime = gameTime.ElapsedGameTime;
            systemManager.ProcessComponents(frameStepTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(screenRT);
            GraphicsDevice.Clear(new Color(94, 109, 119));
            systemManager.DrawEntities(spriteBatch);
            GraphicsDevice.SetRenderTarget(null);

            // Draw render target area to window
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, 
                DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw((Texture2D)screenRT, Vector2.Zero, null, Color.White, 0f, 
                Vector2.Zero, virtualResolutionRatio, SpriteEffects.None, 0f);    

            // Draw debug data
            systemManager.DebugEntities(GraphicsDevice.Viewport, spriteBatch, pixel);
            spriteBatch.DrawString(debugFont, systemManager.totalEntities.ToString(),
                new Vector2(2, GraphicsDevice.Viewport.Height - 24f), Color.White);    
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
