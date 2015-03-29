using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Bummerman.Components;
using Meteor.ECS;

namespace Bummerman
{
    class Level
    {
        int gridLength = 15;
        int gridHeight = 13;
        int maxPlayerBombs = 9;

        /// <summary>
        /// Load all level entities
        /// </summary>
        public void Load(ComponentManager componentManager)
        {
            LoadTiles(componentManager);
        }

        /// <summary>
        /// Load tile entities
        /// </summary>
        private void LoadTiles(ComponentManager componentManager)
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
                        solidBlock = componentManager.CreateEntityFromTemplate("SolidBlock");
                    }
                    else
                    {
                        // Add inner blocks
                        if (x % 2 == 0 && y % 2 == 0)
                            solidBlock = componentManager.CreateEntityFromTemplate("SolidBlock");
                    }

                    // Update solid blocks
                    if (solidBlock != null)
                    {
                        TilePosition tilePos = (TilePosition)solidBlock.GetComponent((int)ComponentType.TilePosition);
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
                                EntityTemplate softBlock = componentManager.CreateEntityFromTemplate("SoftBlock");

                                TilePosition tilePos = (TilePosition)softBlock.GetComponent((int)ComponentType.TilePosition);
                                tilePos.position = new Point(x, y);
                            }
                        }
                    }
                    // Finished adding this block
                }
            }

            // Level camera parameters
            Vector3 camPosition = new Vector3(168, 350, 300);
            Vector3 camLookAt = new Vector3(168, 0, 160);
            float fov = MathHelper.PiOver4;

            // Add a camera to view the level with
            EntityTemplate camera = new EntityTemplate(
                "Template",
                new Components.ScreenPosition(),
                new Components.Camera()
                {
                    fieldOfView = fov,
                    position = camPosition,
                    lookAt = camLookAt,
                    view = Matrix.CreateLookAt(camPosition, camLookAt, Matrix.Identity.Up),
                    projection = Matrix.CreatePerspectiveFieldOfView(fov, 16/9f, 1, 1000)
                }
            );
            componentManager.CreateEntityFromTemplate(camera);
        }

        /// <summary>
        /// Set controls unique to each player
        /// </summary>
        /// <param name="player"></param>
        private void SetPlayerControls(EntityTemplate player)
        {
            InputContext inputContext = (InputContext)player.GetComponent((int)ComponentType.InputContext);
            PlayerInfo playerInfo = (PlayerInfo)player.GetComponent((int)ComponentType.PlayerInfo);

            // Default mappings to keys
            Keys[] keyMappings = new Keys[] { Keys.Q, Keys.E, Keys.A, Keys.D, Keys.S, Keys.W };

            int playerNumber = playerInfo.playerNumber - 1;
            int i = 0;

            // Set the right mappings for this player
            inputContext.SetInputs(
                new KeyValuePair<Keys, InputActions>[]
                {
                    new KeyValuePair<Keys, InputActions>(keyMappings[i++], InputActions.setBomb),
                    new KeyValuePair<Keys, InputActions>(keyMappings[i++], InputActions.remoteTrigger),
                }, 
                new KeyValuePair<Keys, InputStates>[]
                {
                    new KeyValuePair<Keys, InputStates>(keyMappings[i++], InputStates.MoveLeft),
                    new KeyValuePair<Keys, InputStates>(keyMappings[i++], InputStates.MoveRight),
                    new KeyValuePair<Keys, InputStates>(keyMappings[i++], InputStates.MoveDown),
                    new KeyValuePair<Keys, InputStates>(keyMappings[i++], InputStates.MoveUp),
                }
            );
        }

        /// <summary>
        /// Load player entity
        /// </summary>
        public void LoadPlayer(ComponentManager componentManager, int activePlayer)
        {
            Vector2[] startingPositions = 
                { new Vector2(24, 30), new Vector2(208, 8), new Vector2(16, 120), new Vector2(208, 120) };

            //for (int i = 0; i < numberOfPlayers; i++)
            //{
                EntityTemplate player = componentManager.CreateEntityFromTemplate("Player");
                ScreenPosition screenPos = (ScreenPosition)player.GetComponent((int)ComponentType.ScreenPosition);
                PlayerInfo playerInfo = (PlayerInfo)player.GetComponent((int)ComponentType.PlayerInfo);
                
                playerInfo.playerNumber = activePlayer;
                screenPos.position = startingPositions[activePlayer - 1];

                // Set controls for active player
                SetPlayerControls(player);

                // Pre-load bomb entities for each player (maximum carrying capacity)
                for (int j = 0; j < maxPlayerBombs; j++)
                {
                    EntityTemplate playerBomb = componentManager.CreateEntityFromTemplate("Bomb");
                    Bomb bomb = (Bomb)playerBomb.GetComponent((int)ComponentType.Bomb);
                    bomb.ownerID = activePlayer;
                }
            //}
            // Finish loading players
        }
    }
}
