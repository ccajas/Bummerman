using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman
{
    /// <summary>
    /// Keeps track of level objects with Tile components.
    /// </summary>
    class TileSystem : EntitySystem
    {
        // Local components
        Components.TilePosition[] tilePos;
        Components.ScreenPosition[] screenPos;

        // Constants
        readonly int levelTileSize = 16;

        /// <summary>
        /// Constructor to add components
        /// </summary>
        public TileSystem(Components.TilePosition[] tilePositions,
            Components.ScreenPosition[] screenPositions)
        {
            // Initialize component lists
            this.tilePos = tilePositions;
            this.screenPos = screenPositions;
        }

        /// <summary>
        /// Align and update screen tiles
        /// </summary>
        public override void Process(TimeSpan frameStepTime, int totalEntities)
        {
            for (int i = 0; i < totalEntities; i++)
            {
                if (tilePos[i] != null)
                {
                    screenPos[i].position.X = tilePos[i].position.X * levelTileSize;
                    screenPos[i].position.Y = tilePos[i].position.Y * levelTileSize;
                }
            }

            base.Process(frameStepTime, totalEntities);
        }
    }
}
