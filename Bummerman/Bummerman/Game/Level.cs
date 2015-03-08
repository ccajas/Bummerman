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
        int maxPlayerBombs = 9;
        int gridLength = 15;
        int gridHeight = 13;

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
            Random rnd = new Random(123);

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridLength; x++)
                {
                    EntityTemplate solidBlock = null;

                    if (x == 0 || x == gridLength - 1 || y == 0 || y == gridHeight - 1)
                    {
                        // Add border blocks
                        solidBlock = entityManager.CreateEntityFromTemplate("SolidBlock");
                    }
                    else
                    {
                        // Add inner blocks
                        if (x % 2 == 0 && y % 2 == 0)
                            solidBlock = entityManager.CreateEntityFromTemplate("SolidBlock");
                    }

                    // Update solid blocks
                    if (solidBlock != null)
                    {
                        TilePosition tilePos = (TilePosition)solidBlock.GetComponent(ComponentType.TilePosition);
                        tilePos.position = new Point(x, y);
                    }
                    else
                    {
                        // Randomly place soft blocks in empty areas
                        int rndInt = rnd.Next(100);
                        if (rndInt > 55)
                        {
                            if ((x > 2 && x <= gridLength - 4) || (y > 2 && y <= gridHeight - 4))
                            {
                                EntityTemplate softBlock = entityManager.CreateEntityFromTemplate("SoftBlock");

                                TilePosition tilePos = (TilePosition)softBlock.GetComponent(ComponentType.TilePosition);
                                tilePos.position = new Point(x, y);
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
            int numberOfPlayers = 4;

            Vector2[] startingPositions = 
                { new Vector2(16, 8), new Vector2(208, 8), new Vector2(16, 120), new Vector2(208, 120) };

            for (int i = 0; i < numberOfPlayers; i++)
            {
                EntityTemplate player = entityManager.CreateEntityFromTemplate("Player");
                ScreenPosition screenPos = (ScreenPosition)player.GetComponent(ComponentType.ScreenPosition);
                PlayerInfo playerInfo = (PlayerInfo)player.GetComponent(ComponentType.PlayerInfo);

                playerInfo.playerNumber = i + 1;
                screenPos.position = startingPositions[i];

                // Pre-load bomb entities for each player (maximum carrying capacity)
                for (int j = 0; j < maxPlayerBombs; j++)
                {
                    EntityTemplate playerBomb = entityManager.CreateEntityFromTemplate("Bomb");
                    Bomb bomb = (Bomb)playerBomb.GetComponent(ComponentType.Bomb);
                    bomb.ownerID = i + 1;
                }
            }
            // Finish loading players
        }
    }
}
