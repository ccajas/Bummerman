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
                new Components.TilePosition(),
                new Components.Sprite()
                {
                    spriteTexture = "blocks",
                    textureArea = new Rectangle(0, 0, 16, 16)
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
                new Components.TilePosition(),
                new Components.Sprite()
                {
                    spriteTexture = "blocks",
                    textureArea = new Rectangle(16, 0, 16, 16)
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
                    playerNumber = 0,
                    speed = 75
                },
                new Components.InputContext(
                    new KeyValuePair<Keys, InputActions>[]
                    {
                        new KeyValuePair<Keys, InputActions>(Keys.Space, InputActions.setBomb),
                        new KeyValuePair<Keys, InputActions>(Keys.Q, InputActions.setBomb),
                        new KeyValuePair<Keys, InputActions>(Keys.Enter, InputActions.remoteTrigger),
                    },
                    new KeyValuePair<Keys, InputStates>[]
                    {
                        new KeyValuePair<Keys, InputStates>(Keys.Left, InputStates.MoveLeft),
                        new KeyValuePair<Keys, InputStates>(Keys.Right, InputStates.MoveRight),
                        new KeyValuePair<Keys, InputStates>(Keys.Down, InputStates.MoveDown),
                        new KeyValuePair<Keys, InputStates>(Keys.Up, InputStates.MoveUp),
                        new KeyValuePair<Keys, InputStates>(Keys.A, InputStates.MoveLeft),
                        new KeyValuePair<Keys, InputStates>(Keys.D, InputStates.MoveRight),
                        new KeyValuePair<Keys, InputStates>(Keys.S, InputStates.MoveDown),
                        new KeyValuePair<Keys, InputStates>(Keys.W, InputStates.MoveUp),
                    }
                ),
                new Components.ScreenPosition()
                {
                    layer = 1
                },
                new Components.TilePosition(),
                new Components.Sprite()
                {
                    spriteTexture = "player",
                    textureArea = new Rectangle(0, 0, 16, 16)
                },
                new Components.Collision()
                {
                    collisionType = CollisionType.Player,
                    bounds = new Rectangle(0, 0, 14, 14),
                    offset = new Point(1, 1)
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
                new Components.TilePosition(),
                new Components.Sprite()
                {
                    live = false,
                    spriteTexture = "blocks",
                    textureArea = new Rectangle(0, 16, 16, 16)
                }
                /*new Components.Collision()
                {
                    collisionType = CollisionType.SolidBlock,
                    bounds = new Rectangle(0, 0, 16, 16)
                }*/
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
                    spriteTexture = "blocks",
                    textureArea = new Rectangle(32, 16, 16, 16)
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
