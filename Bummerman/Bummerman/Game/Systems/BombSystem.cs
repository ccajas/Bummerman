using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Bummerman.Systems
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
            Message message = GetMessage(MessageType.InputAction1);

            int playerEntityID = -1;
            for (int i = 0; i < totalEntities; i++)
            {
                // Find entity ID of player
                Components.PlayerInfo info = components.playerInfo[i];

                if (info != null && info.playerNumber == 0)
                    playerEntityID = i;

                // Handle bomb setting
                if (message.messageID == Convert.ToInt16(InputActions.setBomb))
                {
                    if (components.bomb[i] != null && canPlace(i, playerEntityID))
                    {
                        // Enable the bomb
                        components.bomb[i].live = true;
                        components.sprite[i].live = true;

                        // Set its position and add it to list of locations
                        components.tilePosition[i].position = components.tilePosition[playerEntityID].position;
                        bombLocations.Add(components.tilePosition[i].position);
                    }
                }

                // Handle remote trigger
                if (message.messageID == Convert.ToInt16(InputActions.remoteTrigger))
                {
                    // Expire the bomb for this player
                    if (components.bomb[i] != null && components.bomb[i].live)
                        components.timedEffect[i].elapsed = 0f;
                }

                // Check if any live bombs have expired
                if (components.bomb[i] != null && components.bomb[i].live)
                {
                    Components.TimedEffect bombTimer = components.timedEffect[i];
                    bombTimer.elapsed -= (float)frameStepTime.TotalSeconds;

                    // If timer expired, remove this bomb and reset timer
                    if (bombTimer.elapsed <= 0f)
                    {
                        bombTimer.elapsed = 5f;
                        components.bomb[i].live = false;
                        components.sprite[i].live = false;

                        // Place explosion
                    }
                }
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

            Point playerPosition = components.tilePosition[playerEntityID].position;

            if (bombLocations.Find(item => item == playerPosition) != Point.Zero)
                return false;

            return true;
        }
    }
}
