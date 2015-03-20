using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace Bummerman.ScreenElements
{
    /// <summary>
    /// Game mode that sets up a server to host a game
    /// </summary>
    class ServerGameScreen : GameScreen
    {
        // Networking resources
        readonly NetServer networkServer;

        /// <summary>
        /// Setup game server
        /// </summary>
        public ServerGameScreen(Game game, ScreenElement previousScreenElement) :
            base(game, previousScreenElement)
        {
            // Set server port
            NetPeerConfiguration Config = new NetPeerConfiguration("game");
            Config.Port = 14242;

            // Max client amount
            Config.MaximumConnections = 200;
            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            // Set up and start server
            networkServer = new NetServer(Config);
            networkServer.Start();

            // Aww yeah!
            Console.WriteLine("Server Started");
            networkMessage = "Server started!";
        }

        /// <summary>
        /// Handle incoming network messages
        /// </summary>
        public override ScreenElement Update(TimeSpan frameStepTime)
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
                        String approved = "Approved new connection and updated the world status";
                        Console.WriteLine(approved);
                        networkMessage = approved;

                        break;
                    // Data type is all messages manually sent from client
                    // ( Approval is automated process )
                    case NetIncomingMessageType.Data:

                        // Read first byte
                        byte firstByte = im.ReadByte();

                        Console.WriteLine(firstByte);
                        networkMessage = firstByte.ToString();
                        break;

                    default:

                        String noMessage = "No message";

                        Console.WriteLine(noMessage);
                        networkMessage = noMessage;
                        break;
                }

                this.networkServer.Recycle(im);
            }
            // End server test stuff

            // Update Game Screen
            return base.Update(frameStepTime);
        }
    }
}
