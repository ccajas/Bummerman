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
        List<Components.Sprite> spriteComponents;
        List<Components.ScreenPosition> screenPositionComponents;

        // Sprite-Position pairs
        Dictionary<int, Tuple<Components.Sprite, Components.ScreenPosition>> spriteEntities;

        public SpriteRenderSystem(Dictionary<string, Texture2D> textureCollection,
            List<Components.Sprite> spriteComponents, 
            List<Components.ScreenPosition> screenPosComponents)
        {
            // Initialize component lists
            this.textureCollection = textureCollection;
            this.spriteComponents = spriteComponents;
            this.screenPositionComponents = screenPosComponents;

            this.spriteEntities = new Dictionary<int, Tuple<Components.Sprite, Components.ScreenPosition>>();
        }

        public override void Process()
        {

        }

        public override void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {           
            foreach (Components.Sprite sprite in spriteComponents)
            {

            }
        }
    }
}
