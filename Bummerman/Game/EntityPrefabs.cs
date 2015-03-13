using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman
{
    static class EntityPrefabs
    {
        /// <summary>
        /// Solid block prefab
        /// </summary>
        public static EntityTemplate CreateSolidBlock(int entityID = -1)
        {
            EntityTemplate template = new EntityTemplate(
                "Template",
                new Components.ScreenPosition(),
                new Components.TilePosition() { tileSize = 16 },
                new Components.Sprite()
                {
                    spriteTexture = "sprites",
                    textureArea = new Rectangle(33, 290, 16, 16)
                },
                new Components.Collision()
                {
                    collisionType = CollisionType.SolidBlock,
                    bounds = new Rectangle(0, 0, 16, 16)
                }
            );

            return SetComponentEntityIDs(template, entityID);
        }

        /// <summary>
        /// Soft block prefab
        /// </summary>
        public static EntityTemplate CreateSoftBlock(int entityID = -1)
        {
            EntityTemplate template = new EntityTemplate(
                "Template",
                new Components.ScreenPosition(),
                new Components.TilePosition() { tileSize = 16 },
                new Components.Sprite()
                {
                    spriteTexture = "sprites",
                    textureArea = new Rectangle(16, 290, 16, 16)
                },
                new Components.Collision()
                {
                    collisionType = CollisionType.SoftBlock,
                    bounds = new Rectangle(0, 0, 16, 16)
                }
            );

            return SetComponentEntityIDs(template, entityID);
        }

        /// <summary>
        /// Player prefab
        /// </summary>
        public static EntityTemplate CreatePlayer(int entityID = -1)
        {
            EntityTemplate template = new EntityTemplate(
                "Template",
                new Components.PlayerInfo()
                {
                    playerNumber = 1,
                    speed = 75
                },
                // Player controls are set in the Level class
                new Components.InputContext(),
                new Components.ScreenPosition()
                {
                    offset = new Point (1, 10),
                    layer = 1
                },
                new Components.TilePosition(),
                new Components.Sprite()
                {
                    spriteTexture = "sprites",
                    textureArea = new Rectangle(68, 38, 16, 24),
                    animation = Animation.None,
                    frameLength = 0.2f,
                    frameCount = 3
                },
                new Components.Collision()
                {
                    collisionType = CollisionType.Player,
                    bounds = new Rectangle(0, 0, 14, 14),
                    offset = new Point(1, 10)
                }
            );

            return SetComponentEntityIDs(template, entityID);
        }

        /// <summary>
        /// Bomb prefab
        /// </summary>
        public static EntityTemplate CreateBomb(int entityID = -1)
        {
            EntityTemplate template = new EntityTemplate(
                "Template",
                new Components.Bomb()               { live = false },
                new Components.TimedEffect()        { elapsed = 5f },
                new Components.ScreenPosition(),
                new Components.TilePosition()       { tileSize = 16 },
                new Components.Sprite()
                {
                    live = false,
                    spriteTexture = "sprites",
                    textureArea = new Rectangle(50, 256, 16, 16),
                    animation = Animation.DualForward,
                    frameLength = 0.5f,
                    frameCount = 3
                },
                new Components.Collision()
                {
                    collisionType = CollisionType.SemiSolid,
                    bounds = new Rectangle(0, 0, 16, 16)
                }
            );

            return SetComponentEntityIDs(template, entityID);
        }

        /// <summary>
        /// Explosion prefab
        /// </summary>
        public static EntityTemplate CreateExplosion(int entityID = -1)
        {
            EntityTemplate template = new EntityTemplate(
                "Template",
                new Components.Spreadable(),
                new Components.ScreenPosition(),
                new Components.TilePosition()   { tileSize = 16 },
                new Components.TimedEffect() { elapsed = 1.25f },
                new Components.Sprite()
                {
                    spriteTexture = "sprites",
                    textureArea = new Rectangle(518, 290, 16, 16)
                },
                new Components.Collision()
                {
                    collisionType = CollisionType.Explosion,
                    bounds = new Rectangle(0, 0, 16, 16)
                }
            );

            return SetComponentEntityIDs(template, entityID);
        }

        /// <summary>
        /// Power-Ups
        /// </summary>
 
        /// Extra Bomb prefab
        public static EntityTemplate CreatePowerUp_ExtraBomb(int entityID = -1)
        {
            EntityTemplate template = new EntityTemplate(
                "Template",
                new Components.ScreenPosition(),
                new Components.TilePosition()   { tileSize = 16 },
                new Components.PowerUp()        { bombUprade = 1 },
                new Components.Sprite()
                {
                    spriteTexture = "sprites",
                    textureArea = new Rectangle(564, 35, 16, 16)
                },
                new Components.Collision()
                {
                    collisionType = CollisionType.PassThrough,
                    bounds = new Rectangle(0, 0, 16, 16)
                }
            );

            return SetComponentEntityIDs(template, entityID);
        }

        /// <summary>
        /// Set the entity ID for each component here
        /// </summary>
        private static EntityTemplate SetComponentEntityIDs(EntityTemplate template, int ID)
        {
            foreach (Component component in template.componentList)
                component.SetOwnerEntity(ID);

            return template;
        }
    }
}
