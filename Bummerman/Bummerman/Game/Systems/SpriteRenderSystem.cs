using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman
{
    class SpriteRenderSystem : EntitySystem
    {
        Dictionary<string, Texture2D> textureCollection;

        // Local components
        Components.Sprite[] sprites;
        Components.ScreenPosition[] screenPos;

        public SpriteRenderSystem(Dictionary<string, Texture2D> textureCollection,
            Components.Sprite[] spriteComponents, 
            Components.ScreenPosition[] screenPos)
        {
            // Initialize component lists
            this.textureCollection = textureCollection;
            this.sprites = spriteComponents;
            this.screenPos = screenPos;
        }

        public override void Process(TimeSpan frameStepTime, int totalEntities)
        {
            this.totalEntities = totalEntities;
        }

        public override void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            for (int i = 0; i < totalEntities; i++)
            {
                // Check valid Sprite components
                if (sprites[i] != null)
                {
                    // Draw the sprite
                    spriteBatch.Draw(textureCollection[sprites[i].spriteTexture],
                        screenPos[i].position, sprites[i].textureArea, Color.White);
                }
            }
            // Finish drawing entities
            spriteBatch.End();
        }
    }
}
