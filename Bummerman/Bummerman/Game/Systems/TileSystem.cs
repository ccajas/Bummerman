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
        // Constants
        readonly int levelTileSize = 16;

        /// <summary>
        /// Constructor to add components
        /// </summary>
        public TileSystem(ComponentCollection components) : base(components) { }

        /// <summary>
        /// Align and update screen tiles
        /// </summary>
        public override void Process(TimeSpan frameStepTime, int totalEntities)
        {
            Components.ScreenPosition[] screenPos = components.screenPosition;
            Components.TilePosition[] tilePos = components.tilePosition;

            for (int i = 0; i < totalEntities; i++)
            {
                if (components.tilePosition[i] != null)
                {
                    screenPos[i].position.X = tilePos[i].position.X * levelTileSize;
                    screenPos[i].position.Y = tilePos[i].position.Y * levelTileSize;
                }
            }

            base.Process(frameStepTime, totalEntities);
        }
    }
}
