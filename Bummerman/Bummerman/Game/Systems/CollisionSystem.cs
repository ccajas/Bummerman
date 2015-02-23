using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Bummerman.Systems
{
    using ComponentCollection = Dictionary<ComponentType, Component[]>;

    class CollisionSystem : EntitySystem
    {
        List<Components.Collision> playerColliders = new List<Components.Collision>();
        List<Components.Collision> blockColliders = new List<Components.Collision>();

        /// Important components
        Components.Collision[] collision;
        Components.ScreenPosition[] screenPosition;
        Components.TilePosition[] tilePosition;

        /// <summary>
        /// Constructor to add components
        /// </summary>
        public CollisionSystem(ComponentCollection components) : base(components) 
        {
            // Load important components
            collision = components[ComponentType.Collision] as Components.Collision[];
            screenPosition = components[ComponentType.ScreenPosition] as Components.ScreenPosition[];      
            tilePosition = components[ComponentType.TilePosition] as Components.TilePosition[];          
        }

        /// <summary>
        /// Dectect and resolve collision from various entity types
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            this.totalEntities = totalEntities;

            // Update collision info
            for (int i = 0; i < totalEntities; i++)
            {
                if (collision[i] != null)
                {
                    // Add and update player colliders
                    if (collision[i].collisionType == CollisionType.Player)
                    {
                        collision[i].bounds.X = (int)screenPosition[i].position.X;
                        collision[i].bounds.Y = (int)screenPosition[i].position.Y;
                        playerColliders.Add(collision[i]);
                    }

                    // Add and update block colliders
                    if (collision[i].collisionType == CollisionType.SolidBlock ||
                        collision[i].collisionType == CollisionType.SoftBlock)
                    {
                        collision[i].bounds.X = tilePosition[i].position.X * tilePosition[i].tileSize;
                        collision[i].bounds.Y = tilePosition[i].position.Y * tilePosition[i].tileSize;

                        blockColliders.Add(collision[i]);
                    }
                }
            }

            // Collision testing and response
            foreach (Components.Collision playerCollider in playerColliders)
            {
                Rectangle playerBounds = playerCollider.bounds;
                playerBounds.X += playerCollider.offset.X;
                playerBounds.Y += playerCollider.offset.Y;

                foreach (Components.Collision blockCollider in blockColliders)
                {
                    Rectangle blockBounds = blockCollider.bounds;

                    if (playerBounds.Intersects(blockBounds))
                    {
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(playerBounds, blockBounds);

                        // Player collided with block, so resolve the collision
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            Components.ScreenPosition playerPos =
                                components[ComponentType.ScreenPosition][playerCollider.entityID] 
                                as Components.ScreenPosition;
                            Vector2 newPosition = playerPos.position;

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX)
                            {
                                // Resolve the collision along the Y axis.
                                newPosition.Y += depth.Y;
                            }
                            else // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                newPosition.X += depth.X;
                            }

                            // Round to whole numbers
                            newPosition.X = (float)Math.Round(newPosition.X);
                            newPosition.Y = (float)Math.Round(newPosition.Y);
                            playerPos.position = newPosition;

                            // Perform further collisions with the new bounds.
                            playerBounds.X = playerCollider.offset.X + (int)newPosition.X;
                            playerBounds.Y = playerCollider.offset.Y + (int)newPosition.Y;
                        }
                    }                 
                }
                // Finished checking all blocks
            }

            playerColliders.Clear();

            return base.Process(frameStepTime, totalEntities);
        }
    }
}
