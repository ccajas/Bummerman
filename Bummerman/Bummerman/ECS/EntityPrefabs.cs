using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
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
                new Components.TilePosition(),
                new Components.Sprite()
                {
                    spriteTexture = "blocks",
                    textureArea = new Rectangle(0, 0, 16, 16)
                },
                new Components.Collision()
                {
                    collisionType = CollisionType.SolidBlock
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
                new Components.TilePosition(),
                new Components.Sprite()
                {
                    spriteTexture = "blocks",
                    textureArea = new Rectangle(16, 0, 16, 16)
                },
                new Components.Collision()
                {
                    collisionType = CollisionType.SolidBlock
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
                    playerNumber = 0
                },
                new Components.ScreenPosition(),
                new Components.TilePosition(),
                new Components.Sprite()
                {
                    spriteTexture = "player",
                    textureArea = new Rectangle(0, 0, 16, 16)
                },
                new Components.Collision()
                {
                    collisionType = CollisionType.Player
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
