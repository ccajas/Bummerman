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
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            Message message = GetMessage(MessageType.InputAction1);

            int playerEntityID = -1;
            for (int i = 0; i < totalEntities; i++)
            {
                // Find entity ID of player
                Components.PlayerInfo info = components.playerInfo[i];

                // Get bomb data
                Components.Bomb bomb = components.bomb[i];
                Components.Sprite sprite = components.sprite[i];
                Components.TimedEffect bombTimer = components.timedEffect[i];
                Components.TilePosition tile = components.tilePosition[i];

                if (info != null && info.playerNumber == 0)
                    playerEntityID = i;

                // Handle bomb setting
                if (message.messageID == Convert.ToInt16(InputActions.setBomb))
                {
                    if (bomb != null && canPlace(bomb, playerEntityID))
                    {
                        // Enable the bomb
                        bomb.live = true;
                        sprite.live = true;

                        // Set its position and add it to list of locations
                        tile.position = components.tilePosition[playerEntityID].position;
                        bombLocations.Add(tile.position);
                    }
                }

                // Handle remote trigger
                if (message.messageID == Convert.ToInt16(InputActions.remoteTrigger))
                {
                    // Expire the bomb for this player
                    if (bomb != null && bomb.live)
                        bombTimer.elapsed = 0f;
                }

                // Check if any live bombs have expired
                if (bomb != null && bomb.live)
                {
                    bombTimer.elapsed -= (float)frameStepTime.TotalSeconds;

                    // If timer expired, remove this bomb and reset timer
                    if (bombTimer.elapsed <= 0f)
                    {
                        bombTimer.elapsed = 5f;
                        bomb.live = false;
                        sprite.live = false;

                        // Place explosion
                        //components
                    }
                }
            }

            return base.Process(frameStepTime, totalEntities);
        }

        /// <summary>
        /// Check if player can place a bomb
        /// </summary>
        /// <returns></returns>
        private bool canPlace(Components.Bomb bomb, int playerEntityID)
        {
            if (bomb.live || bomb.ownerID != 0)
                return false;

            Point playerPosition = components.tilePosition[playerEntityID].position;

            if (bombLocations.Find(item => item == playerPosition) != Point.Zero)
                return false;

            return true;
        }
    }
}
