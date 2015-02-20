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
        public static EntityTemplate CreateSolidBlock()
        {
            return new EntityTemplate(
                "SolidBlock",
                new Components.TilePosition(),
                new Components.ScreenPosition(),
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
        }

        /// <summary>
        /// Soft block prefab
        /// </summary>
        public static EntityTemplate CreateSoftBlock()
        {
            return new EntityTemplate(
                "SoftBlock",
                new Components.TilePosition(),
                new Components.ScreenPosition(),
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
        }
    }
}
