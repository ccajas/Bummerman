using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Meteor.ECS;

namespace Bummerman.ScreenElements
{
    /// <summary>
    /// This defines the Entity prefabs available for the game
    /// </summary>
    partial class GameScreen
    {
        // Types of entities for this game
        Dictionary<string, EntityTemplate> entityPrefabs = 
            new Dictionary<string, EntityTemplate>
        {
            /// <summary>
            /// Player prefab
            /// </summary>
            {
                "Player",
                new EntityTemplate(
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
                        textureArea = new Rectangle(0, 8, 32, 40),
                        animation = Animation.Forward,
                        frameLength = 0.25f,
                        frameCount = 2
                    },
                    new Components.Collision()
                    {
                        collisionType = CollisionType.Player,
                        bounds = new Rectangle(0, 0, 18, 20),
                        offset = new Point(6, 20)
                    }
                )
            },

            /// <summary>
            /// Solid block prefab
            /// </summary>
            { 
                "SolidBlock",
                new EntityTemplate(
                    "Template",
                    new Components.ScreenPosition(),
                    new Components.TilePosition(),
                    new Components.MeshModel()
                    {
                        modelName = "solidBlock1",
                        effectName = "default"
                    },
                    new Components.Sprite()
                    {
                        spriteTexture = "sprites",
                        textureArea = new Rectangle(68, 0, 24, 24)
                    },
                    new Components.Collision()
                    {
                        collisionType = CollisionType.SolidBlock,
                        bounds = new Rectangle(0, 0, 24, 24)
                    }
                )
            },
            /// <summary>
            /// Soft block prefab
            /// </summary>
            {
                "SoftBlock",
                new EntityTemplate(
                    "Template",
                    new Components.ScreenPosition(),
                    new Components.TilePosition(),
                    new Components.MeshModel()
                    {
                        modelName = "barrel",
                        textureName = "barrel_dm",
                        effectName = "default"
                    },
                    new Components.Sprite()
                    {
                        spriteTexture = "sprites",
                        textureArea = new Rectangle(68, 24, 24, 24)
                    },
                    new Components.Collision()
                    {
                        collisionType = CollisionType.SoftBlock,
                        bounds = new Rectangle(0, 0, 24, 24)
                    }
                )
            },

            /// <summary>
            /// Bomb prefab
            /// </summary>
            {
                "Bomb",
                new EntityTemplate(
                    "Template",
                    new Components.Bomb()               { live = false },
                    new Components.TimedEffect()        { elapsed = 5f },
                    new Components.ScreenPosition(),
                    new Components.TilePosition(),
                    new Components.Sprite()
                    {
                        live = false,
                        spriteTexture = "sprites",
                        textureArea = new Rectangle(92, 24, 24, 24),
                        animation = Animation.None,
                        frameLength = 0.5f,
                        frameCount = 3
                    },
                    new Components.Collision()
                    {
                        collisionType = CollisionType.SemiSolid,
                        bounds = new Rectangle(0, 0, 24, 24)
                    }
                )
            },
            /// <summary>
            /// Explosion prefab
            /// </summary>
            {
                "Explosion",
                new EntityTemplate(
                    "Template",
                    new Components.Spreadable(),
                    new Components.ScreenPosition(),
                    new Components.TilePosition(),
                    new Components.TimedEffect() { elapsed = 1.25f },
                    new Components.Sprite()
                    {
                        spriteTexture = "sprites",
                        textureArea = new Rectangle(92, 0, 24, 24),
                        animation = Animation.None,
                        frameLength = 0.15f,
                        frameCount = 1
                    },
                    new Components.Collision()
                    {
                        collisionType = CollisionType.Explosion,
                        bounds = new Rectangle(0, 0, 24, 24)
                    }
                )
            },
            /// <summary>
            /// Power-Ups
            /// </summary>

            /// Extra Bomb prefab
            {
                "PowerUp_ExtraBomb",
                new EntityTemplate(
                    "Template",
                    new Components.ScreenPosition(),
                    new Components.TilePosition(),
                    new Components.PowerUp()        { bombUpgrade = 1 },
                    new Components.Sprite()
                    {
                        spriteTexture = "sprites",
                        textureArea = new Rectangle(116, 0, 24, 24)
                    },
                    new Components.Collision()
                    {
                        collisionType = CollisionType.PassThrough,
                        bounds = new Rectangle(0, 0, 24, 24)
                    }
                )
            }
        };
        // End Prefab/Template list
    }
}
