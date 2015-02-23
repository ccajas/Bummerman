using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Bummerman.Components;

namespace Bummerman.Systems
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

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public ExplosionSystem(EntityManager entityManager)
            : base(entityManager)
        {
            // Load important components
            tiles = components[ComponentType.TilePosition] as TilePosition[];
            colliders = components[ComponentType.Collision] as Collision[];
            spread = components[ComponentType.Spreadable] as Spreadable[];
            timedEffect = components[ComponentType.TimedEffect] as TimedEffect[];

            explosionsToCheck = new Dictionary<Point, Spreadable>();
            allExplosions = new HashSet<Point>();
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

            // Check for any tiles that would be affected by explosions.
            for (int i = 0; i < totalEntities; i++)
            {
                if (colliders[i] != null && allExplosions.Contains(tiles[i].position))
                {
                    // Apply explosion impacts to soft blocks by removing it from the stage.
                    if (colliders[i].collisionType == CollisionType.SoftBlock)
                    {
                        colliders[i].collisionType = CollisionType.PassThrough;
                        entityMgr.DisableEntity(i);
                    }

                    // Explosions can't pass through solid blocks so remove them
                    if (colliders[i].collisionType == CollisionType.SolidBlock)
                    {
                        explosionsToRemove.Add(tiles[i].position);
                    }
                }
            }

            // Check for explosions that need removal
            for (int i = 0; i < totalEntities; i++)
            {
                if (colliders[i] != null && colliders[i].collisionType == CollisionType.Explosion)
                {
                    // Remove the explosion
                    if (explosionsToRemove.Contains(tiles[i].position) ||
                        (components[ComponentType.TimedEffect][i] as TimedEffect).elapsed <= 0f)
                    {
                        // Set spread to 0 to prevent further explosions
                        spread[i].range = 0;
                        entityMgr.RemoveEntity(i);

                        allExplosions.Remove(tiles[i].position);
                        totalEntities--;
                        i--;
                    }
                }
            }

            explosionsToRemove.Clear();

            return base.Process(frameStepTime, totalEntities);
        }

        private void CreateNewExplosion(KeyValuePair<Point, Spreadable> newExplosion)
        {
            EntityTemplate explosion = entityMgr.CreateEntityFromTemplate("Explosion");
            TilePosition explosionTile = (TilePosition)explosion.GetComponent(ComponentType.TilePosition);
            Spreadable explosionSpread = (Spreadable)explosion.GetComponent(ComponentType.Spreadable);

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
                    Bomb bomb = (components[ComponentType.Bomb][i] as Bomb);
                    if (bomb != null)
                        timedEffect[i].elapsed = 0f;
                }
            }
        }

        /// <summary>
        /// Look for explosion at a given tile position
        /// </summary>
        private Spreadable GetExplosionAt(Point tilePosition)
        {
            for (int i = 0; i < totalEntities; i++)
            {
                if (spread[i] != null && tiles[i].position == tilePosition)
                    return spread[i];
            }

            return null;
        }
    }
}
