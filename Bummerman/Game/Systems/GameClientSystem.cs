﻿using System;
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
        int activePlayer = 0;

        /// <summary>
        /// Constructor to add components
        /// </summary>
        public GameClientSystem(ComponentManager componentManager,
            NetClient networkClient,
            SpriteBatch spriteBatch,
            SpriteFont debugFont)
            : base(componentManager)
        {
            // Load important components           
            inputs = components[(int)ComponentType.InputContext] as InputContext[];
            playerInfo = components[(int)ComponentType.PlayerInfo] as PlayerInfo[];

            this.networkClient = networkClient;
            this.spriteBatch = spriteBatch;
            this.debugFont = debugFont;

            // Default message
            networkMessage = "Waiting to connect...";
        }

        /// <summary>
        /// Send to and receive messages from server
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            // Check for any new player inputs
            for (int i = 0; i < totalEntities; i++)
            {
                if (playerInfo[i] != null && playerInfo[i].live &&
                    playerInfo[i].playerNumber == activePlayer)
                {
                    // Create a test message to send to the server
                    NetOutgoingMessage outmsg = networkClient.CreateMessage();

                    // Send player ID and input messageS if player had pressed anything
                    if (inputs[i].currentState > 0 || inputs[i].currentAction > 0)
                    {
                        outmsg.Write((byte)playerInfo[i].playerNumber);
                        outmsg.Write((byte)inputs[i].currentState);
                        outmsg.Write((byte)inputs[i].currentAction);

                        networkClient.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                    }
                }
            }

            // Create new incoming message holder
            NetIncomingMessage im;

            while ((im = networkClient.ReadMessage()) != null)
            {
                if (im.MessageType == NetIncomingMessageType.Data)
                {
                    if (im.LengthBytes == 1)
                    {
                        // Message data contains no. of connections
                        AddPlayersToLevel(im.ReadByte());
                        networkMessage = "Connected to server";
                    }
                    else
                    {
                        // Message containes player input status
                        byte playerNumber = im.ReadByte();
                        byte playerInputState = im.ReadByte();
                        byte playerInputAction = im.ReadByte();

                        // Have the server update its own player
                        UpdatePlayerData(playerNumber, playerInputState, playerInputAction);
                    }
                }
            }

            return totalEntities;
        }

        /// <summary>
        /// Add player with proper number ID. No. of previous connections determines player number
        /// </summary>
        private void AddPlayersToLevel(byte playerNumber)
        {
            Level level = new Level();
            level.LoadPlayer(componentMgr, (int)playerNumber);
            activePlayer = playerNumber;
        }

        /// <summary>
        /// Update the player data from a server
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
        /// Show debug message output for client
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
