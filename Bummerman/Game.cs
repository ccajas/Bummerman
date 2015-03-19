
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
        SpriteBatch spriteBatch;

        // ECS entity manager
        SystemManager systemManager;

        // Sprite textures and other assets
        Dictionary<string, Texture2D> textureCollection;
        //Dictionary<string, Model> meshCollection;

        RenderTarget2D screenRT;
        SpriteFont debugFont;
        
        // Virtual resolution for adaptive resizing
        int virtualBufferWidth = 428;
        int virtualBufferHeight = 240;

        // Default to virtual res ratio
        float virtualResolutionRatio = 1f;

        // Game resources
        Level level;

        // Networking resources
        readonly NetClient networkClient;
        readonly NetServer networkServer;

        /// <summary>
        /// Game constructor
        /// </summary>
        public BummermanGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Setup server

            // Set server port
            NetPeerConfiguration Config = new NetPeerConfiguration("game");
            Config.Port = 14242;

            // Max client amount
            Config.MaximumConnections = 200;
            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            // Set up and start server
            networkServer = new NetServer(Config);
            networkServer.Start();

            Console.WriteLine("Server Started");

            // Setup client

            // Create new instance of configs. Parameter is "application Id". It has to be same on client and server.
            NetPeerConfiguration Config2 = new NetPeerConfiguration("game");

            networkClient = new NetClient(Config2);
            networkClient.Start();

            // Write byte
            NetOutgoingMessage outmsg = networkClient.CreateMessage();
            outmsg.Write((byte)1);
            outmsg.Write("MyName");

            // Connect client, to ip previously requested from user 
            networkClient.Connect("localhost", 14242, outmsg);
            Console.WriteLine("Client Started");
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
            //meshCollection = new Dictionary<string, Model>();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Add SystemManager and component types to it
            SetupComponentsAndSystems();
            SetupEntityPrefabs();

            // Setup level
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
            // load your game content here
            textureCollection.Add("sprites", Content.Load<Texture2D>("textures/sprites"));

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
        }
        /// <summary>
        /// Creates systems and entity templates
        /// </summary>
        private void SetupComponentsAndSystems()
        {
            int maxEntities = 1000;

            systemManager = new SystemManager(new Dictionary<ComponentType, Component[]>
            { 
                { ComponentType.ScreenPosition, new Components.ScreenPosition[maxEntities]},
                { ComponentType.TilePosition,   new Components.TilePosition[maxEntities]},
                { ComponentType.Sprite,         new Components.Sprite[maxEntities]},
                { ComponentType.InputContext,   new Components.InputContext[maxEntities]},
                { ComponentType.Collision,      new Components.Collision[maxEntities]},
                { ComponentType.PlayerInfo,     new Components.PlayerInfo[maxEntities]},
                { ComponentType.Bomb,           new Components.Bomb[maxEntities]},
                { ComponentType.PowerUp,        new Components.PowerUp[maxEntities]},
                { ComponentType.Spreadable,     new Components.Spreadable[maxEntities]},
                { ComponentType.TimedEffect,    new Components.TimedEffect[maxEntities]}
            });

            systemManager.AddSystems(new EntitySystem[] 
            {
                new InputSystem         (systemManager.Entities),
                new MovementSystem      (systemManager.Entities),
                new BombSystem          (systemManager.Entities),
                new ExplosionSystem     (systemManager.Entities),
                new PowerUpSystem       (systemManager.Entities),
                new TileSystem          (systemManager.Entities),
                new CollisionSystem     (systemManager.Entities),
                new SpriteRenderSystem  (systemManager.Entities,
                    textureCollection, spriteBatch)
            });
        }

        private void SetupEntityPrefabs()
        {
            foreach (KeyValuePair<string, EntityTemplate> entityPrefab in entityPrefabs)
                systemManager.Entities.AddEntityTemplate(entityPrefab.Key, entityPrefab.Value);
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

            NetIncomingMessage im;

            while ((im = this.networkServer.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    // If incoming message is Request for connection approval
                    // This is the very first packet/message that is sent from client
                    // Here you can do new player initialisation stuff
                    case NetIncomingMessageType.ConnectionApproval:

                        // Read the first byte of the packet
                        // ( Enums can be casted to bytes, so it be used to make bytes human readable )
                        Console.WriteLine("Incoming LOGIN");
                        im.SenderConnection.Approve();

                        // Create test message to send
                        NetOutgoingMessage outmsg = networkServer.CreateMessage();
                        outmsg.Write("Test message");
                        networkServer.SendMessage(outmsg, im.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);

                        // Debug
                        Console.WriteLine("Approved new connection and updated the world status");

                        break;
                    // Data type is all messages manually sent from client
                    // ( Approval is automated process )
                    case NetIncomingMessageType.Data:

                        // Read first byte
                        byte firstByte = im.ReadByte();

                        Console.WriteLine(firstByte);
                        break;

                    default:

                        Console.WriteLine("No message");
                        break;
                }

                this.networkServer.Recycle(im);
            }

            // Handle the component and entity updates
            TimeSpan frameStepTime = gameTime.ElapsedGameTime;
            systemManager.ProcessComponents(frameStepTime);

            // Create a test message to send to the server
            NetOutgoingMessage outmsg2 = networkClient.CreateMessage();
            outmsg2.Write((byte)gameTime.TotalGameTime.TotalSeconds);
            outmsg2.Write("Test");

            // Send it to server
            networkClient.SendMessage(outmsg2, NetDeliveryMethod.ReliableOrdered);

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
            systemManager.DrawEntities();
            GraphicsDevice.SetRenderTarget(null);

            // Draw render target area to window
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, 
                DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw((Texture2D)screenRT, Vector2.Zero, null, Color.White, 0f, 
                Vector2.Zero, virtualResolutionRatio, SpriteEffects.None, 0f);
            spriteBatch.End();

            // Draw ECS debug data
            DrawDebugData();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Display debug data from ECS
        /// </summary>
        [Conditional("DEBUG")]
        private void DrawDebugData()
        {
            int[] entityInfo = new int[1];
            systemManager.DebugEntities(ref entityInfo);

            spriteBatch.Begin();
            spriteBatch.Draw(pixel, new Rectangle(0, GraphicsDevice.Viewport.Height - 88, 
                GraphicsDevice.Viewport.Width, 108), new Color(0, 0, 0, 0.8f));
            spriteBatch.Draw(pixel, new Rectangle(4 + (200 * 4), GraphicsDevice.Viewport.Height - 88, 3, 7), Color.White);

            // Default component colors
            Color[] colors = { new Color(0, 0, 0, 0.5f), Color.White, Color.Blue, Color.Cyan, Color.LightGreen, 
                                 Color.Yellow, Color.Green, Color.Red, Color.Orange, Color.Fuchsia, 
                                 new Color(0, 255, 0), Color.LightSkyBlue };

            // Display component info for each entity
            for (int i = 0; i < entityInfo.Length; i++)
            {
                int j = 1;
                int component = 0;
                while (j < (1 << 10))
                {
                    j = 1 << component;
                    int c = ((int)(entityInfo[i] & j) == j) ? component + 1 : 0;

                    spriteBatch.Draw(pixel, new Rectangle(4 + (i * 4),
                        GraphicsDevice.Viewport.Height - 80 + (component * 6), 3, 5), colors[c]);

                    component++;
                }
            }

            spriteBatch.DrawString(debugFont, systemManager.totalEntities.ToString(),
                new Vector2(2, GraphicsDevice.Viewport.Height - 24f), Color.White);
            spriteBatch.End();
        }
    }
}
