#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Bummerman
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BummermanGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // ECS entity manager
        SystemManager systemManager;

        // Sprite textures and other assets
        Dictionary<string, Texture2D> textureCollection;
        Dictionary<string, Model> meshCollection;

        BasicEffect basicEffect;
        RenderTarget2D screenRT;
        SpriteFont debugFont;
        
        // Virtual resolution for adaptive resizing
        int virtualBufferWidth = 428;
        int virtualBufferHeight = 240;

        // Default to virtual res ratio
        float virtualResolutionRatio = 1f;

        // Game resources
        Level level;

        public BummermanGame()
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
            // Add your initialization logic here
            textureCollection = new Dictionary<string, Texture2D>();
            meshCollection = new Dictionary<string, Model>();

            // Add SystemManager and component types to it
            SetupComponentsAndSystems();
            level = new Level();

            // Graphics settings
            graphics.PreferredBackBufferWidth = 854;
            graphics.PreferredBackBufferHeight = 480;
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
            textureCollection.Add("sprites", Content.Load<Texture2D>("textures/sprites"));

            //meshCollection.Add("block1", Content.Load<Model>("models/solidblock1"));

            // Set render target to virtual resolution
            screenRT = new RenderTarget2D(GraphicsDevice,
                virtualBufferWidth,
                virtualBufferHeight
            );
            debugFont = Content.Load<SpriteFont>("debug");
            virtualResolutionRatio = (float)GraphicsDevice.Viewport.Width / (float)virtualBufferWidth;

            // Load level entities
            level.Load(systemManager.Entities);

            // Finished loading content
            // Make a 1x1 texture named pixel.  
            pixel = new Texture2D(GraphicsDevice, 1, 1);

            // Set the texture data with our color information.  
            pixel.SetData<Color>(colorData);
            /*
            // Set up BasicEffect
            basicEffect = new BasicEffect(graphics.GraphicsDevice);

            float tilt = MathHelper.ToRadians(0);  // 0 degree angle
            // Use the world matrix to tilt the cube along x and y axes.
            Matrix worldMatrix = Matrix.CreateRotationX(tilt) * Matrix.CreateRotationY(tilt);
            Matrix viewMatrix = Matrix.CreateLookAt(new Vector3(15, 30, 50), new Vector3(15, 0, 10), Vector3.Up);

            Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),  // 45 degree angle
                (float)GraphicsDevice.Viewport.AspectRatio,
                1.0f, 1000.0f);

            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;

            // primitive color
            basicEffect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);

            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f); // a red light
            basicEffect.DirectionalLight0.Direction = new Vector3(1, -1, 0.4f);  // coming along the x-axis
            basicEffect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0); // with green highlights
            basicEffect.DirectionalLight0.Enabled = true;

            basicEffect.DiffuseColor = Vector3.One;
            basicEffect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
            basicEffect.SpecularPower = 1.0f;
            basicEffect.Alpha = 1.0f;

            basicEffect.LightingEnabled = true;
            */
        }

        // Creates systems and entity templates
        private void SetupComponentsAndSystems()
        {
            systemManager = new SystemManager(new Component[] { });

            systemManager.AddSystems(new EntitySystem[] 
            {
                new Systems.InputSystem     (systemManager.Entities),
                new Systems.MovementSystem  (systemManager.Entities),
                new Systems.BombSystem      (systemManager.Entities),
                new Systems.ExplosionSystem (systemManager.Entities),
                new Systems.PowerUpSystem   (systemManager.Entities),
                new Systems.TileSystem      (systemManager.Entities),
                new Systems.CollisionSystem (systemManager.Entities),
                new Systems.SpriteRenderSystem(basicEffect, meshCollection, 
                    textureCollection, systemManager.Entities)
            });
        }

        // Background pixel texture 
        private Texture2D pixel;
        private Color[] colorData = { Color.White };  

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
            /*
            Vector3 direction = basicEffect.DirectionalLight0.Direction;
            direction.X = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds);
            direction.Z = (float)Math.Cos(gameTime.TotalGameTime.TotalSeconds);
            basicEffect.DirectionalLight0.Direction = direction;
            */
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
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, 
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
