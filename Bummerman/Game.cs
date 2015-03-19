
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
        //SpriteBatch spriteBatch;

        // Networking resources
        readonly NetClient networkClient;
        readonly NetServer networkServer;

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
            graphics.PreferredBackBufferWidth = 854;
            graphics.PreferredBackBufferHeight = 480;
            graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();

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
            currentScreen = new ScreenElements.GameScreen(this, null);
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

            // Create a test message to send to the server
            NetOutgoingMessage outmsg2 = networkClient.CreateMessage();
            outmsg2.Write((byte)gameTime.TotalGameTime.TotalSeconds);
            outmsg2.Write("Test");

            // Send it to server
            networkClient.SendMessage(outmsg2, NetDeliveryMethod.ReliableOrdered);

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
            // Start drawing the ScreenElements
            base.Draw(gameTime);

            // Draw the current screen
            (currentScreen as DrawableScreenElement).Draw(gameTime.ElapsedGameTime);
        }
    }
}
