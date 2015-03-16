using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman
{
    /// <summary>
    /// This defines the Entity prefabs available for the game
    /// </summary>
    public partial class BummermanGame
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
                )
            }
        };
        // End Prefab/Template list
    }
}
