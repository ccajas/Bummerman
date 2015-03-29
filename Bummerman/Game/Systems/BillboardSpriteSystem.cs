using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bummerman.Components;
using Meteor.ECS;

namespace Bummerman
{
    class BillboardSpriteSystem : DrawableEntitySystem
    {
        /// Reference to texture and model assets
        Dictionary<string, Texture2D> textureCollection;
        Dictionary<string, Effect> effectCollection;

        /// Billboard sprite geometry
        VertexPositionTexture[] quadVertices;
        short[] quadIndices;

        /// Important components
        Sprite[] sprites;
        ScreenPosition[] screenPosition;

        GraphicsDevice graphicsDevice;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public BillboardSpriteSystem(ComponentManager componentManager,
            Dictionary<string, Texture2D> textureCollection,
            Dictionary<string, Effect> effectCollection,
            GraphicsDevice graphicsDevice)
            : base(componentManager)
        {
            // Set graphics device
            this.graphicsDevice = graphicsDevice;

            // Initialize asset collections
            this.textureCollection = textureCollection;
            this.effectCollection = effectCollection;

            // Load important components
            sprites = components[(int)ComponentType.Sprite] as Sprite[];
            screenPosition = components[(int)ComponentType.ScreenPosition] as ScreenPosition[];
            
            // Create billboard quad vertices
            quadVertices = new VertexPositionTexture[4]
            {
                new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0))
            };

            // Create the indices
            quadIndices = new short[6] { 0, 1, 2, 2, 3, 0 };
        }

        /// <summary>
        /// Draw entities with billboarded Sprite components
        /// </summary>
        public override void Draw()
        {
            // Setup active camera(s)
            for (int i = 0; i < totalEntities; i++)
            {
                // Check valid Camera compnents
                if (components[(int)ComponentType.Camera][i] != null)
                {
                    // Set global shader variables
                    Effect defaultEffect = effectCollection["default"];
                    Camera camera = components[(int)ComponentType.Camera][i] as Camera;

                    defaultEffect.Parameters["View"].SetValue(camera.view);
                    defaultEffect.Parameters["Projection"].SetValue(camera.projection);
                }
            }

            // Draw any billboard sprites
            for (int i = 0; i < totalEntities; i++)
            {
                if (sprites[i] != null && sprites[i].billboard)
                {

                }
            }
            // Finish drawing all billboard sprites
        }
    }
}
