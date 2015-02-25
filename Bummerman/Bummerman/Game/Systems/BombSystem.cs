using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bummerman.Components;

namespace Bummerman.Systems
{
    using ComponentCollection = Dictionary<ComponentType, Component[]>;

    /// <summary>
    /// Handle placement and update of bombs
    /// </summary>
    class BombSystem : EntitySystem
    {
        /// Important components
        Bomb[] bombs;
        Sprite[] sprites;
        TilePosition[] tilePosition;
        TimedEffect[] timedEffect;

        /// Set of existing bomb locations
        HashSet<Point> bombLocations = new HashSet<Point>();

        /// List of exploded bombs organized by player number
        List<int> explodedPlayerBombs = new List<int>();

        /// <summary>
        /// Constructor to add components
        /// </summary>
        public BombSystem(EntityManager entityManager)
            : base(entityManager) 
        {
            // Load important components           
            bombs = components[ComponentType.Bomb] as Bomb[];
            sprites = components[ComponentType.Sprite] as Sprite[];
            tilePosition = components[ComponentType.TilePosition] as TilePosition[];
            timedEffect = components[ComponentType.TimedEffect] as TimedEffect[];
        }

        /// <summary>
        /// Update and add/remove bombs as needed
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            Message message = GetMessage(MessageType.InputAction1);

            for (int i = 0; i < totalEntities; i++)
            {
                // Get bomb data
                TimedEffect bombTimer = timedEffect[i];
                TilePosition tile = tilePosition[i];
                Bomb bomb = bombs[i];
                Sprite sprite = sprites[i];

                // Handle remote trigger
                if (message.messageID == Convert.ToInt16(InputActions.remoteTrigger))
                {
                    // Expire the bomb for this player
                    if (bomb != null && bomb.live)
                        bombTimer.elapsed = 0f;
                }

                // Handle bomb setting
                if (message.messageID == Convert.ToInt16(InputActions.setBomb))
                {
                    // Get player data
                    int playerEntityID = (int)message.data;
                    PlayerInfo playerInfo = components[ComponentType.PlayerInfo][playerEntityID] as PlayerInfo;

                    if (bomb != null && canPlace(bomb, playerInfo))
                    {
                        // Enable the bomb and reset timer
                        bombTimer.elapsed = 5f;
                        bomb.live = true;
                        sprite.live = true;

                        // Set its position and add it to list of locations
                        tile.position = tilePosition[playerEntityID].position;
                        bombLocations.Add(tile.position);
                        playerInfo.currentBombs++;   
                    }
                }

                // Check if any live bombs have expired
                if (bomb != null && bomb.live)
                {
                    bombTimer.elapsed -= (float)frameStepTime.TotalSeconds;

                    // If timer expired, remove this bomb
                    if (bombTimer.elapsed <= 0f)
                    {
                        bomb.live = false;
                        sprite.live = false;

                        // Remove bomb location
                        bombLocations.Remove(tile.position);

                        // Place explosion
                        EntityTemplate explosion = entityMgr.CreateEntityFromTemplate("Explosion");
                        TilePosition explosionTile = (TilePosition)explosion.GetComponent(ComponentType.TilePosition);
                        Spreadable explosionSpread = (Spreadable)explosion.GetComponent(ComponentType.Spreadable);

                        explosionTile.position = tile.position;
                        explosionSpread.range = bomb.power;

                        explodedPlayerBombs.Add(bomb.ownerID);
                    }
                }
            }

            // Check for any exploded bombs
            for (int i = 0; i < totalEntities; i++)
            {
                if (components[ComponentType.PlayerInfo][i] != null)
                {
                    PlayerInfo info = components[ComponentType.PlayerInfo][i] as PlayerInfo;
                    int playerNumber = info.playerNumber;

                    for (int j = 0; j < explodedPlayerBombs.Count; j++)
                    {
                        // Give player back an extra bomb
                        if (explodedPlayerBombs[j] == playerNumber)
                            if (info.currentBombs > 0) info.currentBombs--;
                    }
                }
            }

            explodedPlayerBombs.Clear();

            return base.Process(frameStepTime, totalEntities);
        }

        /// <summary>
        /// Check if player can place a bomb
        /// </summary>
        /// <returns></returns>
        private bool canPlace(Components.Bomb bomb, Components.PlayerInfo playerInfo)
        {
            if (bomb.live || bomb.ownerID != playerInfo.playerNumber)
                return false;

            if (playerInfo.currentBombs >= playerInfo.maxBombs)
                return false;

            Point playerPosition = (components[ComponentType.TilePosition][playerInfo.entityID] 
                as TilePosition).position;

            // Player is already in front of a bomb
            if (bombLocations.Contains(playerPosition))
                return false;

            return true;
        }
    }
}
