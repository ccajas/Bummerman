using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;
using Bummerman.Components;
using Meteor.ECS;

namespace Bummerman.Systems
{
    class GameClientSystem : DrawableEntitySystem
    {
        /// Important components
        InputContext[] inputs;
        PlayerInfo[] playerInfo;

        /// Networking resources
        readonly NetClient networkClient;

        /// Graphics resources
        SpriteBatch spriteBatch;
        SpriteFont debugFont;

        /// Debugging network status
        String networkMessage;

        /// <summary>
        /// Constructor to add components
        /// </summary>
        public GameClientSystem(ComponentManager componentManager,
            SpriteBatch spriteBatch,
            SpriteFont debugFont)
            : base(componentManager)
        {
            // Load important components           
            inputs = components[ComponentType.InputContext] as InputContext[];
            playerInfo = components[ComponentType.PlayerInfo] as PlayerInfo[];

            this.spriteBatch = spriteBatch;
            this.debugFont = debugFont;

            // Set player ID
            //activePlayer = 1;

            // Create new instance of configs. Parameter is "application Id". It has to be same on client and server.
            NetPeerConfiguration Config = new NetPeerConfiguration("game");

            networkClient = new NetClient(Config);
            networkClient.Start();

            // Write byte
            NetOutgoingMessage outmsg = networkClient.CreateMessage();
            outmsg.Write((byte)1);
            outmsg.Write("MyName");

            // Connect client, to ip previously requested from user 
            networkClient.Connect("localhost", 14242, outmsg);
            Console.WriteLine("Client Started");

            // Default message
            networkMessage = "Waiting to connect...";
        }

        /// <summary>
        /// Send to and receive messages from server
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            // Create a test message to send to the server
            NetOutgoingMessage outmsg = networkClient.CreateMessage();
            outmsg.Write((byte)frameStepTime.TotalMilliseconds);

            // Send it to server
            networkClient.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);

            // Create new incoming message holder
            NetIncomingMessage im;

            while ((im = networkClient.ReadMessage()) != null)
            {
                if (im.MessageType == NetIncomingMessageType.Data)
                    networkMessage = im.ReadString();
            }

            return totalEntities;
        }

        /// <summary>
        /// Show debug message output for client
        /// </summary>
        public override void Draw()
        {
            // Debug network data stuff here
            spriteBatch.Begin();
            spriteBatch.DrawString(debugFont, networkMessage, new Vector2(2, 0), Color.White);
            spriteBatch.End();
        }
    }
}
