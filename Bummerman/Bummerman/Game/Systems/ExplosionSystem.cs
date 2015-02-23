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

        /// Potential explosion spots
        Dictionary<Point, int> explosionsToCheck;
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

            explosionsToCheck = new Dictionary<Point, int>();
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
                    (components[ComponentType.TimedEffect][i] as TimedEffect).elapsed -= (float)frameStepTime.TotalSeconds;

                    if (spread[i].range > 0)
                    {
                        spread[i].range--;
                        int newRange = spread[i].range;

                        // Try to place new Explosions in all four directions
                        explosionsToCheck[new Point(explosionPos.X + 1, explosionPos.Y)] = newRange;
                        explosionsToCheck[new Point(explosionPos.X, explosionPos.Y + 1)] = newRange;
                        explosionsToCheck[new Point(explosionPos.X - 1, explosionPos.Y)] = newRange;
                        explosionsToCheck[new Point(explosionPos.X, explosionPos.Y - 1)] = newRange;      
                    }
                }
            }
            
            // Create new explosions
            foreach (KeyValuePair<Point, int> location in explosionsToCheck)
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
                        explosionsToRemove.Add(tiles[i].position);
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

        private void CreateNewExplosion(KeyValuePair<Point, int> newExplosion)
        {
            EntityTemplate explosion = entityMgr.CreateEntityFromTemplate("Explosion");
            TilePosition explosionTile = (TilePosition)explosion.GetComponent(ComponentType.TilePosition);
            Spreadable explosionSpread = (Spreadable)explosion.GetComponent(ComponentType.Spreadable);

            explosionTile.position = newExplosion.Key;
            explosionSpread.range = newExplosion.Value;
        }
    }
}
