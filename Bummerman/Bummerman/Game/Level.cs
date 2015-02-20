using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Bummerman.Components;

namespace Bummerman
{
    class Level
    {
        /// <summary>
        /// Load all level entities
        /// </summary>
        public void Load(EntityManager entityManager)
        {
            LoadTiles(entityManager);
            LoadPlayers(entityManager);
        }

        /// <summary>
        /// Load tile entities
        /// </summary>
        private void LoadTiles(EntityManager entityManager)
        {
            // Create a Bomberman-style stage
            int gridLength = 15;
            int gridHeight = 13;

            Random rnd = new Random(123);

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridLength; x++)
                {
                    EntityTemplate solidBlock = null;

                    if (x == 0 || x == gridLength - 1 || y == 0 || y == gridHeight - 1)
                    {
                        // Add border blocks
                        solidBlock = entityManager.CreateEntity("SolidBlock");
                    }
                    else
                    {
                        // Add inner blocks
                        if (x % 2 == 0 && y % 2 == 0)
                            solidBlock = entityManager.CreateEntity("SolidBlock");
                    }

                    // Update solid blocks
                    if (solidBlock != null)
                    {
                        ScreenPosition screenPos = (ScreenPosition)solidBlock.GetComponent(ComponentType.ScreenPosition);
                        screenPos.position = new Vector2(x, y) * 16f;
                        screenPos.layer = 1;
                    }
                    else
                    {
                        // Randomly place soft blocks in empty areas
                        int rndInt = rnd.Next(100);
                        if (rndInt > 45)
                        {
                            if ((x > 2 && x <= gridLength - 4) || (y > 2 && y <= gridHeight - 4))
                            {
                                EntityTemplate softBlock = entityManager.CreateEntity("SoftBlock");

                                ScreenPosition screenPos = (ScreenPosition)softBlock.GetComponent(ComponentType.ScreenPosition);
                                screenPos.position = new Vector2(x, y) * 16f;
                                screenPos.layer = 1;
                            }
                        }
                    }

                    // Finished adding this block
                }
            }
        }

        /// <summary>
        /// Load player entities
        /// </summary>
        private void LoadPlayers(EntityManager entityManager)
        {

        }
    }
}
