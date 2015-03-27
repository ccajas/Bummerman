using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bummerman.Components;
using Meteor.ECS;

namespace Bummerman
{
    class PowerUpSystem : EntitySystem
    {
        /// Important components
        PowerUp[] powerUps;
        PlayerInfo[] playerInfo;
        TilePosition[] tiles;

        /// Set of existing PowerUp locations
        HashSet<Point> powerUpLocations = new HashSet<Point>();

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public PowerUpSystem(ComponentManager componentManager)
            : base(componentManager)
        {
            // Load important components
            powerUps = components[(int)ComponentType.PowerUp] as PowerUp[];
            playerInfo = components[(int)ComponentType.PlayerInfo] as PlayerInfo[];
            tiles = components[(int)ComponentType.TilePosition] as TilePosition[];
        }

        /// <summary>
        /// Handle picking up of powerups and updating Player stats
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            // Check for any powerups and add them to the positions list
            for (int i = 0; i < totalEntities; i++)
            {
                if (powerUps[i] != null)
                {
                    powerUpLocations.Add(tiles[i].position);
                }
            }

            // Only check power-ups when player is overlapping one
            for (int i = 0; i < totalEntities; i++)
            {
                if (playerInfo[i] != null && playerInfo[i].live)
                {
                    // If player collided with a PowerUp, 
                    // apply it to the player and remove it from the stage.
                    if (powerUpLocations.Contains(tiles[i].position))
                        ApplyPowerUp(playerInfo[i]);            
                }
            }

            powerUpLocations.Clear();

            return base.Process(frameStepTime, totalEntities);
        }

        /// <summary>
        /// Let the player pick this powerup
        /// </summary>
        private void ApplyPowerUp(PlayerInfo playerInfo)
        {
            // Loop through possible powerups
            for (int i = 0; i < totalEntities; i++)
            {
                if (powerUps[i] != null && tiles[i].position == tiles[playerInfo.entityID].position)
                {
                    if (playerInfo.maxBombs < 9)
                        playerInfo.maxBombs += powerUps[i].bombUpgrade;

                    if (playerInfo.bombPower < 9)
                        playerInfo.bombPower += powerUps[i].powerUpgrade;

                    // Remove powerup
                    componentMgr.DisableEntity(i);
                }
            }
            // Finish applying powerups
        }
    }
}
