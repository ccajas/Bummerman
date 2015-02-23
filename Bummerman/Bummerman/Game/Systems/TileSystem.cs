using System;
using System.Collections.Generic;

namespace Bummerman.Systems
{
    using ComponentCollection = Dictionary<ComponentType, Component[]>;

    /// <summary>
    /// Keeps track of level objects with Tile components.
    /// </summary>
    class TileSystem : EntitySystem
    {
        // Constants
        readonly int levelTileSize = 16;

        /// Important components
        Components.ScreenPosition[] screenPos;
        Components.TilePosition[] tilePos;

        /// <summary>
        /// Constructor to add components
        /// </summary>
        public TileSystem(EntityManager entityManager)
            : base(entityManager) 
        {
            // Load important components
            screenPos = components[ComponentType.ScreenPosition] as Components.ScreenPosition[];
            tilePos = components[ComponentType.TilePosition] as Components.TilePosition[];    
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
                    if (components[ComponentType.PlayerInfo][i] == null)
                    {
                        // Snap screen position according to their tile location
                        screenPos[i].position.X = tilePos[i].position.X * levelTileSize;
                        screenPos[i].position.Y = tilePos[i].position.Y * levelTileSize;
                    }
                    else
                    {
                        // Player entities can move freely, but still need the closest tile location
                        tilePos[i].position.X = (int)Math.Round(screenPos[i].position.X / (float)levelTileSize);
                        tilePos[i].position.Y = (int)Math.Round(screenPos[i].position.Y / (float)levelTileSize);
                    }
                }
            }

            return base.Process(frameStepTime, totalEntities);
        }
    }
}
