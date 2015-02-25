using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bummerman.Components;

namespace Bummerman.Systems
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
        public PowerUpSystem(EntityManager entityManager)
            : base(entityManager)
        {
            // Load important components
            powerUps = components[ComponentType.PowerUp] as PowerUp[];
            playerInfo = components[ComponentType.PlayerInfo] as PlayerInfo[];
            tiles = components[ComponentType.TilePosition] as TilePosition[];
        }

        /// <summary>
        /// Handle picking up of powerups and updating Player stats
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            Message message = GetMessage(MessageType.InputState1);

            // Only check power-ups when player is moving
            if (message.messageID >= Convert.ToInt16(InputStates.MoveUp))
            {
                for (int i = 0; i < totalEntities; i++)
                {
                    //if (powerUps[i] != null)
                    //    powerUpLocations.Add(
                }
            }

            return base.Process(frameStepTime, totalEntities);
        }
    }
}
