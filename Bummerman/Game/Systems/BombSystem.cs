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
        TimedEffect[] timedEffect;
        PlayerInfo[] playerInfo;

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
            timedEffect = components[ComponentType.TimedEffect] as TimedEffect[];
            playerInfo = components[ComponentType.PlayerInfo] as PlayerInfo[];
        }

        /// <summary>
        /// Update and add/remove bombs as needed
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            // Check for any player bomb actions
            for (int i = 0; i < totalEntities; i++)
            {
                if (playerInfo[i] != null && playerInfo[i].live)
                {
                    InputContext input = components[ComponentType.InputContext][i] as InputContext;

                    if (input.currentAction == Convert.ToInt16(InputActions.remoteTrigger))
                        RemoteTriggerBombs(playerInfo[i].playerNumber);

                    if (input.currentAction == Convert.ToInt16(InputActions.setBomb))
                        SetBomb(playerInfo[i]);
                }
            }

            // Check through all bombs
            for (int i = 0; i < totalEntities; i++)
            {
                // Get bomb data
                TimedEffect bombTimer = timedEffect[i];
                Bomb bomb = bombs[i];
                Sprite sprite = sprites[i];

                // Count down timer for bombs
                if (bomb != null && bomb.live)
                {
                    bombTimer.elapsed -= (float)frameStepTime.TotalSeconds;

                    // If timer expired, remove this bomb
                    if (bombTimer.elapsed <= 0f)
                    {
                        bomb.live = false;
                        sprite.live = false;

                        // Remove bomb location
                        TilePosition tile = components[ComponentType.TilePosition][i] as TilePosition;
                        bombLocations.Remove(tile.position);
                        explodedPlayerBombs.Add(bomb.ownerID);

                        // Place explosion
                        EntityTemplate explosion = entityMgr.CreateEntityFromTemplate("Explosion");
                        TilePosition explosionTile = (TilePosition)explosion.GetComponent(ComponentType.TilePosition);
                        Spreadable explosionSpread = (Spreadable)explosion.GetComponent(ComponentType.Spreadable);

                        explosionTile.position = tile.position;
                        explosionSpread.range = bomb.power;
                    }
                }
            }

            // Check for any exploded bombs
            for (int i = 0; i < totalEntities; i++)
            {
                if (playerInfo[i] != null)
                {
                    int playerNumber = playerInfo[i].playerNumber;

                    for (int j = 0; j < explodedPlayerBombs.Count; j++)
                    {
                        // Give player back an extra bomb
                        if (explodedPlayerBombs[j] == playerNumber)
                            if (playerInfo[i].currentBombs > 0) playerInfo[i].currentBombs--;
                    }
                }
            }

            explodedPlayerBombs.Clear();

            return base.Process(frameStepTime, totalEntities);
        }

        /// <summary>
        /// Detonate all bombs set by this player
        /// </summary>
        private void RemoteTriggerBombs(int playerNumber)
        {
            for (int i = 0; i < totalEntities; i++)
            {
                // Get bomb data
                TimedEffect bombTimer = timedEffect[i];
                Bomb bomb = bombs[i];
                Sprite sprite = sprites[i];

                // Handle remote trigger
                if (bomb != null && bomb.ownerID == playerNumber && bomb.live)
                    bombTimer.elapsed = 0f;
            }
        }

        /// <summary>
        /// Let player set one bomb
        /// </summary>
        private void SetBomb(PlayerInfo playerInfo)
        {
            for (int i = 0; i < totalEntities; i++)
            {
                // Get bomb data
                TimedEffect bombTimer = timedEffect[i];
                Bomb bomb = bombs[i];
                Sprite sprite = sprites[i];

                // Handle bomb setting
                if (bomb != null && CanPlace(bomb, playerInfo))
                {
                    // Enable the bomb and reset timer
                    bombTimer.elapsed = 5f;
                    bomb.live = true;
                    sprite.live = true;

                    // Set its position and add it to list of locations
                    TilePosition tile = components[ComponentType.TilePosition][i] as TilePosition;
                    TilePosition playerTile = 
                        components[ComponentType.TilePosition][playerInfo.entityID] as TilePosition;

                    tile.position = playerTile.position;
                    bombLocations.Add(tile.position);
                    playerInfo.currentBombs++;   
                }
            }
        }

        /// <summary>
        /// Check if player can place a bomb
        /// </summary>
        /// <returns></returns>
        private bool CanPlace(Components.Bomb bomb, Components.PlayerInfo playerInfo)
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
