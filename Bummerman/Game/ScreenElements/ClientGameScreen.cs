using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace Bummerman.ScreenElements
{
    /// <summary>
    /// Game mode that sets up a client to connect to a game server
    /// </summary>
    class ClientGameScreen : GameScreen
    {
        // Networking resources
        readonly NetClient networkClient;

        /// <summary>
        /// Setup game client
        /// </summary>
        public ClientGameScreen(Game game, ScreenElement previousScreenElement) :
            base(game, previousScreenElement)
        {
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
        }

        /// <summary>
        /// Send outgoing messages to server
        /// </summary>
        public override ScreenElement Update(TimeSpan frameStepTime)
        {
            // Create a test message to send to the server
            NetOutgoingMessage outmsg = networkClient.CreateMessage();
            outmsg.Write((byte)frameStepTime.TotalMilliseconds);
            outmsg.Write("Test");

            // Send it to server
            networkClient.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);

            // Update Game Screen
            return base.Update(frameStepTime);
        }
    }
}
