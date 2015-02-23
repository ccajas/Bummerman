using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman.Systems
{
    using ComponentCollection = Dictionary<ComponentType, Component[]>;

    /// <summary>
    /// Renders all entities with a Sprite component.
    /// </summary>
    class SpriteRenderSystem : EntitySystem
    {
        /// Reference to texture assets
        Dictionary<string, Texture2D> textureList;

        /// Important components
        Components.Sprite[] sprites;
        Components.ScreenPosition[] screenPosition;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public SpriteRenderSystem(Dictionary<string, Texture2D> textureList,
            ComponentCollection components) : base(components)
        {
            // Initialize component lists
            this.textureList = textureList;

            // Load important components
            sprites = components[ComponentType.Sprite] as Components.Sprite[];
            screenPosition = components[ComponentType.ScreenPosition] as Components.ScreenPosition[];
        }

        /// <summary>
        /// Process entities
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            return base.Process(frameStepTime, totalEntities);
        }

        /// <summary>
        /// Draw entities with Sprite components
        /// </summary>
        public override void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            for (int i = 0; i < totalEntities; i++)
            {
                // Check valid Sprite components
                if (sprites[i] != null && sprites[i].live)
                {
                    // Draw the sprite
                    spriteBatch.Draw(textureList[sprites[i].spriteTexture],
                        screenPosition[i].position, sprites[i].textureArea, Color.White);
                }
            }
            // Finish drawing entities
            spriteBatch.End();
        }
    }
}
