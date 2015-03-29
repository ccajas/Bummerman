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
            Effect billboardEffect = effectCollection["billboard"];
            Vector3 camPos, camLookAt;

            // Setup active camera(s)
            for (int i = 0; i < totalEntities; i++)
            {
                // Check valid Camera compnents
                if (components[(int)ComponentType.Camera][i] != null)
                {
                    // Set global shader variables
                    Camera camera = components[(int)ComponentType.Camera][i] as Camera;
                    camPos = camera.position;
                    camLookAt = camera.lookAt;

                    billboardEffect.Parameters["View"].SetValue(camera.view);
                    billboardEffect.Parameters["Projection"].SetValue(camera.projection);
                }
            }

            // Draw any billboard sprites
            for (int i = 0; i < totalEntities; i++)
            {
                if (sprites[i] != null && sprites[i].billboard)
                {
                    string textureName = sprites[i].spriteTexture ?? "default";
                    Vector2 position = screenPosition[i].position;
                    Vector2 scale = new Vector2(sprites[i].textureArea.Width, sprites[i].textureArea.Height);

                    // Set texture area coordinates
                    Vector2 finalPos = new Vector2(sprites[i].textureArea.X, sprites[i].textureArea.Y);
                    Vector2 finalSize = new Vector2(sprites[i].textureArea.Width, sprites[i].textureArea.Height);

                    finalPos.X += sprites[i].frame * sprites[i].textureArea.Width;
                    finalPos /= new Vector2((float)sprites[i].textureSize.X, (float)sprites[i].textureSize.Y);
                    finalSize /= new Vector2((float)sprites[i].textureSize.X, (float)sprites[i].textureSize.Y);

                    // Set quad vertices
                    quadVertices[0].TextureCoordinate = new Vector2(finalPos.X + finalSize.X, finalPos.Y + finalSize.Y);
                    quadVertices[1].TextureCoordinate = new Vector2(finalPos.X, finalPos.Y + finalSize.Y);
                    quadVertices[2].TextureCoordinate = new Vector2(finalPos.X, finalPos.Y);
                    quadVertices[3].TextureCoordinate = new Vector2(finalPos.X + finalSize.X, finalPos.Y);

                    // Set model parameters
                    billboardEffect.Parameters["World"].SetValue(
                        Matrix.CreateScale(new Vector3(scale / 2f, 1)) * 
                        Matrix.CreateTranslation(new Vector3(position.X, 0, position.Y)));
                    billboardEffect.Parameters["Texture"].SetValue(textureCollection[textureName]);
                    billboardEffect.CurrentTechnique.Passes[0].Apply();

                    // Draw the sprite
                    graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, quadVertices, 0, 4, quadIndices, 0, 2);
                }
            }
            // Finish drawing all billboard sprites
        }
    }
}
