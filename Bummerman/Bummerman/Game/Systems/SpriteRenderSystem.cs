using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman
{
    /// <summary>
    /// Renders all entities with a Sprite component.
    /// </summary>
    class SpriteRenderSystem : EntitySystem
    {
        Dictionary<string, Texture2D> textureList;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public SpriteRenderSystem(Dictionary<string, Texture2D> textureList,
            ComponentCollection collection) : base(collection)
        {
            // Initialize component lists
            this.textureList = textureList;
        }

        /// <summary>
        /// Process entities
        /// </summary>
        public override void Process(TimeSpan frameStepTime, int totalEntities)
        {
            base.Process(frameStepTime, totalEntities);
        }

        /// <summary>
        /// Draw entities with Sprite components
        /// </summary>
        public override void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Components.Sprite[] sprites = components.sprite;
            Components.ScreenPosition[] screenPos = components.screenPosition;

            spriteBatch.Begin();

            for (int i = 0; i < totalEntities; i++)
            {
                // Check valid Sprite components
                if (sprites[i] != null)
                {
                    // Draw the sprite
                    spriteBatch.Draw(textureList[sprites[i].spriteTexture],
                        screenPos[i].position, sprites[i].textureArea, Color.White);
                }
            }
            // Finish drawing entities
            spriteBatch.End();
        }
    }
}
