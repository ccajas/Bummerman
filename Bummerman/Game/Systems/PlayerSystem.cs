﻿using System;
using Microsoft.Xna.Framework.Graphics;
using Bummerman.Components;
using Meteor.ECS;

namespace Bummerman
{
    /// <summary>
    /// Keeps track of players' total scores and how many are remaining
    /// </summary>
    class PlayerSystem : EntitySystem
    {
        /// Important components
        PlayerInfo[] playerInfo;

        /// Total player support
        int maxPlayers = 2;

        /// <summary>
        /// Constructor to add components
        /// </summary>
        public PlayerSystem(ComponentManager componentManager)
            : base(componentManager) 
        {
            // Load important components
            playerInfo = components[(int)ComponentType.PlayerInfo] as PlayerInfo[];
        }

        /// <summary>
        /// Check up on player stats
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            // Process input data from all possible players
            for (int player = 0; player < maxPlayers; player++)
            {
                // Perform actions if message ID isn't 0
                /*if (message.messageID > 0)
                {
                    int entity = (int)message.receiver;

                    // Check for any dead players
                    if (!playerInfo[entity].live && playerInfo[entity].playerNumber == player + 1)
                    {                      
                        // Play knocked out animation
                    }
                } */
            }

            return base.Process(frameStepTime, totalEntities);
        }

        /// <summary>
        /// Draw player stats HUD on the screen
        /// </summary>
        //public override void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        //{

        //}
    }
}
