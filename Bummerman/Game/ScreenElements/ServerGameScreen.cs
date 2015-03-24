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
    /// Wrapper for Game Screen that sets up a server to host a game
    /// </summary>
    class ServerGameScreen : GameScreen
    {
        /// Game server instance
        NetServer networkServer;

        /// <summary>
        /// Setup game server
        /// </summary>
        public ServerGameScreen(Game game, ScreenElement previousScreenElement) :
            base(game, previousScreenElement)
        {
            // Default message
            networkMessage = "";

            // Set server port
            NetPeerConfiguration Config = new NetPeerConfiguration("game");
            Config.Port = 14242;

            // Max client amount
            Config.MaximumConnections = 32;
            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            // Set up and start server
            networkServer = new NetServer(Config);
            networkServer.Start();
            Console.WriteLine("Server Started");

            // Add a GameServer System to the ECS
            systemManager.AddSystems(new EntitySystem[] 
            { 
                new Systems.GameServerSystem(systemManager.Entities, networkServer, spriteBatch, debugFont) 
            });    
        }

        /// <summary>
        /// Properly shut down server
        /// </summary>
        public override void UnloadContent() 
        { 
            // TODO: Send message prior to shutdown

            networkServer.Shutdown("Shutting down");
        }
    }
}
