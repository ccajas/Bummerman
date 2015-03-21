using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using Meteor.ECS;

namespace Bummerman.ScreenElements
{
    /// <summary>
    /// Wrapper for Game Screen that sets up a client to connect to a game server
    /// </summary>
    class ClientGameScreen : GameScreen
    {
        /// <summary>
        /// Setup game client
        /// </summary>
        public ClientGameScreen(Game game, ScreenElement previousScreenElement) :
            base(game, previousScreenElement)
        {
            // Default message
            networkMessage = "";

            // Create new instance of configs. Parameter is "application Id". It has to be same on client and server.
            NetPeerConfiguration Config = new NetPeerConfiguration("game");

            NetClient networkClient = new NetClient(Config);
            networkClient.Start();

            // Write byte
            NetOutgoingMessage outmsg = networkClient.CreateMessage();
            outmsg.Write((byte)1);
            outmsg.Write("test");

            string ipString = NetUtility.Resolve("localhost").ToString();

            // Connect client, to ip previously requested from user 
            networkClient.Connect("localhost", 14242, outmsg);
            Console.WriteLine("Client Started");

            // Add a GameClient System to the ECS
            systemManager.AddSystems(new EntitySystem[] 
            { 
                new Systems.GameClientSystem(systemManager.Entities, networkClient, spriteBatch, debugFont) 
            });
        }
    }
}
