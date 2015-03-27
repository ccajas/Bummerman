using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;
using Bummerman.Components;
using Meteor.ECS;

namespace Bummerman.Systems
{
    class GameServerSystem : DrawableEntitySystem
    {
        /// Important components
        InputContext[] inputs;
        PlayerInfo[] playerInfo;

        /// Networking resources
        readonly NetServer networkServer;

        /// Graphics resources
        SpriteBatch spriteBatch;
        SpriteFont debugFont;

        /// Hosted game information
        String networkMessage;
        byte totalPlayers = 1;
        byte currentPlayerList = 1;

        /// <summary>
        /// Constructor to add components
        /// </summary>
        public GameServerSystem(ComponentManager componentManager, 
            NetServer networkServer,
            SpriteBatch spriteBatch, 
            SpriteFont debugFont)
            : base(componentManager) 
        {
            // Load important components           
            inputs = components[(int)ComponentType.InputContext] as InputContext[];
            playerInfo = components[(int)ComponentType.PlayerInfo] as PlayerInfo[];

            this.networkServer = networkServer;
            this.spriteBatch = spriteBatch;
            this.debugFont = debugFont;

            // Host player gets automatically added
            AddPlayerToLevel(totalPlayers);

            // Default message
            networkMessage = "Server started!";
        }

        /// <summary>
        /// Handle incoming network messages
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            // Check incoming messages
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

                        // Update player count to all players
                        byte playerCount = ++totalPlayers;

                        // Add new player based on current connection count
                        AddPlayerToLevel(playerCount);

                        NetOutgoingMessage outmsg = networkServer.CreateMessage();
                        outmsg.Write(playerCount);
                        networkServer.SendMessage(outmsg, im.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                        // Debug
                        Console.WriteLine("Approved new connection and updated the world status");
                        Console.WriteLine("Total players: " + playerCount.ToString());

                        break;

                    // Data type is all messages manually sent from client
                    // ( Approval is automated process )
                    case NetIncomingMessageType.Data:

                        // Read the bytes
                        byte playerNumber = im.ReadByte();
                        byte playerInputState = im.ReadByte();
                        byte playerInputAction = im.ReadByte();

                        // Have the server update other players
                        UpdatePlayerData(playerNumber, playerInputState, playerInputAction);

                        networkMessage = "Player "+ playerNumber.ToString() + " input: " + 
                            playerInputState.ToString();
                        break;

                    default:

                        String noMessage = "No message";

                        Console.WriteLine(noMessage);
                        networkMessage = noMessage;
                        break;
                }

                this.networkServer.Recycle(im);
            }

            byte connectionC = (byte)networkServer.Connections.Count;

            /*
            // Send outgoing messages back
            for (int i = 0; i < totalEntities; i++)
            {
                if (playerInfo[i] != null && playerInfo[i].live &&
                    playerInfo[i].playerNumber == activePlayer)
                {
                    // Create a test message to send to the server
                    NetOutgoingMessage outmsg = networkServer.CreateMessage();

                    // Send player ID and input messageS if player had pressed anything
                    if (inputs[i].currentState > 0 || inputs[i].currentAction > 0)
                    {
                        outmsg.Write((byte)playerInfo[i].playerNumber);
                        outmsg.Write((byte)inputs[i].currentState);
                        outmsg.Write((byte)inputs[i].currentAction);

                        // Let clients know how many connections are active
                        outmsg.Write((byte)networkServer.Connections.Count);

                        networkServer.SendMessage(outmsg, networkServer.Connections,
                            NetDeliveryMethod.ReliableOrdered, 0);
                    }
                }
            } */

            return totalEntities;
        }

        /// <summary>
        /// Add player with proper number ID. No. of previous connections determines player number
        /// </summary>
        private void AddPlayerToLevel(byte playerNumber)
        {
            Level level = new Level();
            level.LoadPlayer(componentMgr, (int)playerNumber);
        }

        /// <summary>
        /// Update the player data from a client
        /// </summary>
        private void UpdatePlayerData(byte playerNumber, byte playerInputState, byte playerInputAction)
        {
            // Look for player inputs
            for (int i = 0; i < totalEntities; i++)
            {
                if (playerInfo[i] != null && playerInfo[i].live &&
                    playerInfo[i].playerNumber == playerNumber)
                {
                    inputs[i].currentState = (uint)playerInputState;
                    inputs[i].currentAction = (uint)playerInputAction;
                    inputs[i].updatedByServer = true;
                }
                // Finished updating this input
            }
        }

        /// <summary>
        /// Show debug message output for server
        /// </summary>
        public override void Draw()
        {
            // Debug network data stuff here
            spriteBatch.Begin();
            spriteBatch.DrawString(debugFont, networkMessage, new Vector2(2, 1), Color.Black);
            spriteBatch.DrawString(debugFont, networkMessage, new Vector2(2, 0), Color.White);
            spriteBatch.End();
        }
    }
}
