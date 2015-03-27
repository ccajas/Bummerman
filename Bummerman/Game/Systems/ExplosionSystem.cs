using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Bummerman.Components;
using Meteor.ECS;

namespace Bummerman
{
    /// <summary>
    /// Updates explosions and handles their effects
    /// </summary>
    class ExplosionSystem : EntitySystem
    {
        /// Important components
        TilePosition[] tiles;
        Collision[] colliders;
        Spreadable[] spread;
        TimedEffect[] timedEffect;

        /// Potential explosion spots
        Dictionary<Point, Spreadable> explosionsToCheck;
        HashSet<Point> allExplosions;
        HashSet<Point> softBlockLocations;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public ExplosionSystem(ComponentManager componentManager)
            : base(componentManager)
        {
            // Load important components
            tiles = components[(int)ComponentType.TilePosition] as TilePosition[];
            colliders = components[(int)ComponentType.Collision] as Collision[];
            spread = components[(int)ComponentType.Spreadable] as Spreadable[];
            timedEffect = components[(int)ComponentType.TimedEffect] as TimedEffect[];

            // Setup location containers
            explosionsToCheck = new Dictionary<Point, Spreadable>();
            allExplosions = new HashSet<Point>();
            softBlockLocations = new HashSet<Point>();
        }

        /// <summary>
        /// Process explosions
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            // First, list the explosions found on the stage.
            for (int i = 0; i < totalEntities; i++)
            {
                if (colliders[i] != null && colliders[i].collisionType == CollisionType.Explosion)
                {
                    Point explosionPos = tiles[i].position;
                    allExplosions.Add(explosionPos);

                    // Count down timer
                    timedEffect[i].elapsed -= (float)frameStepTime.TotalSeconds;

                    // For explosions with spread left, create new explosions
                    if (spread[i].range > 0)
                    {
                        int newRange = spread[i].range - 1;
                        spread[i].range = 0;

                        // Try to place new Explosions in all four directions
                        if (spread[i].direction == Direction.None || spread[i].direction == Direction.Left)
                            explosionsToCheck[new Point(explosionPos.X - 1, explosionPos.Y)] =
                                new Spreadable { range = newRange, direction = Direction.Left };

                        if (spread[i].direction == Direction.None || spread[i].direction == Direction.Top)
                            explosionsToCheck[new Point(explosionPos.X, explosionPos.Y - 1)] =
                                new Spreadable { range = newRange, direction = Direction.Top };

                        if (spread[i].direction == Direction.None || spread[i].direction == Direction.Right)
                            explosionsToCheck[new Point(explosionPos.X + 1, explosionPos.Y)] = 
                                new Spreadable { range = newRange, direction = Direction.Right };

                        if (spread[i].direction == Direction.None || spread[i].direction == Direction.Bottom)
                            explosionsToCheck[new Point(explosionPos.X, explosionPos.Y + 1)] =
                                new Spreadable { range = newRange, direction = Direction.Bottom };
                    }
                }
            }
            
            // Create new explosions
            foreach (KeyValuePair<Point, Spreadable> location in explosionsToCheck)
            {
                allExplosions.Add(location.Key);
                CreateNewExplosion(location);
            }

            // Set new explosion temps
            explosionsToCheck.Clear();
            HashSet<Point> explosionsToRemove = new HashSet<Point>();

            // Check for any tiles or players that would be affected by explosions
            for (int i = 0; i < totalEntities; i++)
            {
                if (colliders[i] != null && colliders[i].live && allExplosions.Contains(tiles[i].position))
                {
                    // Apply explosion impacts to soft blocks and power-ups by removing them from the stage
                    if (colliders[i].collisionType == CollisionType.SoftBlock)
                    {
                        componentMgr.DisableEntity(i);

                        // Store soft block location for later
                        softBlockLocations.Add(tiles[i].position);
                    }

                    // Explosions can't pass through solid blocks so remove them
                    if (colliders[i].collisionType == CollisionType.SolidBlock)
                        explosionsToRemove.Add(tiles[i].position);

                    // Remove any other non-solid objects (that includes players)
                    if (colliders[i].collisionType == CollisionType.PassThrough)
                        componentMgr.DisableEntity(i);

                    if (colliders[i].collisionType == CollisionType.Player)
                        components[(int)ComponentType.PlayerInfo][i].live = false;
                }
            }

            // Check for explosions that need removal
            for (int i = 0; i < totalEntities; i++)
            {
                if (colliders[i] != null && colliders[i].collisionType == CollisionType.Explosion)
                {
                    // Remove the explosion
                    Point tilePosition = tiles[i].position;
                    if (explosionsToRemove.Contains(tilePosition) || timedEffect[i].elapsed <= 0f)
                    {
                        componentMgr.DisableEntity(i);
                        allExplosions.Remove(tilePosition);

                        // If this explosion was on a Soft Block, there is a chance of revealing a powerup
                        if (softBlockLocations.Contains(tilePosition))
                        {
                            softBlockLocations.Remove(tilePosition);
                            Random r = new Random();

                            int random = r.Next(100);
                            if (random < 100)
                                CreateNewPowerUp(tilePosition, random);
                        }
                    }
                    // Finished removing explosion
                }
            }

            explosionsToRemove.Clear();

            return base.Process(frameStepTime, totalEntities);
        }

        /// <summary>
        /// Create an explosion at a given location with defined parameters
        /// </summary>
        private void CreateNewExplosion(KeyValuePair<Point, Spreadable> newExplosion)
        {
            EntityTemplate explosion = componentMgr.CreateEntityFromTemplate("Explosion");
            TilePosition explosionTile = (TilePosition)explosion.GetComponent((int)ComponentType.TilePosition);
            Spreadable explosionSpread = (Spreadable)explosion.GetComponent((int)ComponentType.Spreadable);

            explosionTile.position = newExplosion.Key;
            explosionSpread.range = newExplosion.Value.range;
            explosionSpread.direction = newExplosion.Value.direction;

            // Loop through all possible collidable entities
            for (int i = 0; i < totalEntities; i++)
            {
                if (colliders[i] != null && tiles[i].position == newExplosion.Key)
                {
                    // Weaken explosions when they hit blocks
                    if (colliders[i].collisionType != CollisionType.PassThrough)
                        explosionSpread.range = 0;

                    // Bombs hit by explosion should detonate
                    Bomb bomb = (components[(int)ComponentType.Bomb][i] as Bomb);
                    if (bomb != null)
                        timedEffect[i].elapsed = 0f;
                }
            }
        }

        /// <summary>
        /// Create a randomized power-up
        /// </summary>
        private void CreateNewPowerUp(Point tileLocation, int randomValue)
        {
            EntityTemplate powerUp = componentMgr.CreateEntityFromTemplate("PowerUp_ExtraBomb");
            TilePosition powerUpTile = (TilePosition)powerUp.GetComponent((int)ComponentType.TilePosition);

            powerUpTile.position = tileLocation;
        }
    }
}
