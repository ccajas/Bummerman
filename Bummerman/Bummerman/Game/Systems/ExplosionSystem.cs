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

        /// Potential explosion spots
        HashSet<Point> explosionsToCheck;
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

            explosionsToCheck = new HashSet<Point>();
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

                    if ((components[ComponentType.TimedEffect][i] as TimedEffect).elapsed <= 0)
                    {
                        (components[ComponentType.TimedEffect][i] as TimedEffect).elapsed = 10000f;

                        // Try to place new Explosions in all four directions
                        explosionsToCheck.Add(new Point(explosionPos.X + 1, explosionPos.Y));
                        explosionsToCheck.Add(new Point(explosionPos.X, explosionPos.Y + 1));
                    }
                }
            }
            
            // Create new explosions
            foreach (Point location in explosionsToCheck)
                CreateNewExplosion(location);

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

                    // Also tag the explosion for removal
                    //explosionsToRemove.Add(tiles[i].position);
                }
            }

            // Check for explosions that need removal
            for (int i = 0; i < totalEntities; i++)
            {
                if (colliders[i] != null && colliders[i].collisionType == CollisionType.Explosion)
                {
                    // Remove the explosion
                    if (explosionsToRemove.Contains(tiles[i].position))
                    {
                        entityMgr.RemoveEntity(i);
                        allExplosions.Remove(tiles[i].position);
                    }
                }
            }

            explosionsToRemove.Clear();

            return base.Process(frameStepTime, totalEntities);
        }

        private void CreateNewExplosion(Point newPosition)
        {
            EntityTemplate explosion = entityMgr.CreateEntityFromTemplate("Explosion");
            TilePosition explisionTile = (TilePosition)explosion.GetComponent(ComponentType.TilePosition);
            explisionTile.position = newPosition;
            explisionTile.tileSize = 16;
        }
    }
}
