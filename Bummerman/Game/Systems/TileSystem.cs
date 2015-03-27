using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bummerman.Components;
using Meteor.ECS;

namespace Bummerman
{
    using ComponentCollection = Dictionary<ComponentType, Component[]>;

    /// <summary>
    /// Keeps track of level objects with Tile components.
    /// </summary>
    class TileSystem : EntitySystem
    {
        /// Constants
        readonly int levelTileSize = 24;

        /// Important components
        ScreenPosition[] screenPos;
        TilePosition[] tilePos;

        /// <summary>
        /// Constructor to add components
        /// </summary>
        public TileSystem(ComponentManager componentManager)
            : base(componentManager) 
        {
            // Load important components
            screenPos = components[(int)ComponentType.ScreenPosition] as ScreenPosition[];
            tilePos = components[(int)ComponentType.TilePosition] as TilePosition[];    
        }

        /// <summary>
        /// Align and update screen tiles
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            for (int i = 0; i < totalEntities; i++)
            {
                if (tilePos[i] != null)
                {
                    if (components[(int)ComponentType.PlayerInfo][i] == null)
                    {
                        // Snap screen position according to their tile location
                        screenPos[i].position.X = tilePos[i].position.X * tilePos[i].tileSize;
                        screenPos[i].position.Y = tilePos[i].position.Y * tilePos[i].tileSize;
                    }
                    else
                    {
                        Vector2 playerPos;
                        playerPos.X = screenPos[i].position.X + screenPos[i].offset.X;
                        playerPos.Y = screenPos[i].position.Y + screenPos[i].offset.Y;

                        // Player entities can move freely, but still need the closest tile location
                        tilePos[i].position.X = (int)Math.Round(playerPos.X / (float)levelTileSize);
                        tilePos[i].position.Y = (int)Math.Round(playerPos.Y / (float)levelTileSize);
                    }
                }
            }

            return base.Process(frameStepTime, totalEntities);
        }
    }
}
