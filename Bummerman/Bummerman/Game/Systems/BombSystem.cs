﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bummerman
{
    class BombSystem : EntitySystem
    {
        /// List of existing bomb locations
        private List<Point> bombLocations = new List<Point>();

        /// <summary>
        /// Constructor to add components
        /// </summary>
        public BombSystem(ComponentCollection components) : base(components) { }

        /// <summary>
        /// Update and add/remove bombs as needed
        /// </summary>
        public override void Process(TimeSpan frameStepTime, int totalEntities)
        {
            Message message = GetMessage(MessageType.Player1Action);

            // Handle bomb setting first
            if (message.messageID == Convert.ToInt16(InputActions.setBomb))
            {
                int playerEntityID = -1;
                for (int i = 0; i < totalEntities; i++)
                {
                    // Find entity ID of player
                    Components.PlayerInfo info = components.playerInfo[i];

                    if (info != null && info.playerNumber == 0)
                        playerEntityID = i;

                    if (components.bomb[i] != null && canPlace(i, playerEntityID))
                    {
                        // Enable the bomb
                        components.bomb[i].live = true;
                        components.sprite[i].live = true;

                        // Set its position and add it to list of locations
                        components.tilePosition[i].position = components.tilePosition[playerEntityID].position;
                        bombLocations.Add(components.tilePosition[i].position);

                        break;
                    }
                }

                message.messageID = 0;
            }

            base.Process(frameStepTime, totalEntities);
        }

        /// <summary>
        /// Check if player can place a bomb
        /// </summary>
        /// <returns></returns>
        private bool canPlace(int entity, int playerEntityID)
        {
            if (components.bomb[entity].live || components.bomb[entity].ownerID != 0)
                return false;

            Point found = bombLocations.Find(item => 
                item == components.tilePosition[playerEntityID].position);

            if (found != Point.Zero)
                return false;

            return true;
        }
    }
}
