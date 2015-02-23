using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Bummerman.Systems
{
    using ComponentCollection = Dictionary<ComponentType, Component[]>;

    /// <summary>
    /// Handle placement and update of bombs
    /// </summary>
    class BombSystem : EntitySystem
    {
        /// List of existing bomb locations
        private List<Point> bombLocations = new List<Point>();

        /// Important components
        Components.Bomb[] bombs;
        Components.Sprite[] sprites;
        Components.TilePosition[] tilePosition;
        Components.TimedEffect[] timedEffect;

        /// <summary>
        /// Constructor to add components
        /// </summary>
        public BombSystem(ComponentCollection components) : base(components) 
        {
            // Load important components           
            bombs = components[ComponentType.Bomb] as Components.Bomb[];
            sprites = components[ComponentType.Sprite] as Components.Sprite[];
            tilePosition = components[ComponentType.TilePosition] as Components.TilePosition[];
            timedEffect = components[ComponentType.TimedEffect] as Components.TimedEffect[];
        }

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
                Components.PlayerInfo info = components[ComponentType.PlayerInfo][i] 
                    as Components.PlayerInfo;

                // Get bomb data
                Components.TimedEffect bombTimer = timedEffect[i];
                Components.TilePosition tile = tilePosition[i];
                Components.Bomb bomb = bombs[i];
                Components.Sprite sprite = sprites[i];

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
                        tile.position = tilePosition[playerEntityID].position;
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
                        bombLocations.Remove(tile.position);
                        //EntityPrefabs.CreatePlayer(++totalEntities);
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

            Point playerPosition = (components[ComponentType.TilePosition][playerEntityID] 
                as Components.TilePosition).position;

            if (bombLocations.Find(item => item == playerPosition) != Point.Zero)
                return false;

            return true;
        }
    }
}
