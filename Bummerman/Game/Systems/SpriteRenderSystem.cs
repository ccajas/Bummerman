using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bummerman.Components;

namespace Bummerman.Systems
{
    using ComponentCollection = Dictionary<ComponentType, Component[]>;

    /// <summary>
    /// Renders all entities with a Sprite component.
    /// </summary>
    class SpriteRenderSystem : EntitySystem
    {
        /// Reference to texture and model assets
        Dictionary<string, Texture2D> textureCollection;
        Dictionary<string, Model> modelCollection;

        /// Basic Effect for rendering
        BasicEffect basicEffect;

        /// Offset position
        Vector2 screenAreaOffset = new Vector2(92, 8);

        /// Important components
        Sprite[] sprites;
        ScreenPosition[] screenPosition;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public SpriteRenderSystem(BasicEffect basicEffect, 
            Dictionary<string, Model> modelCollection,
            Dictionary<string, Texture2D> textureCollection, EntityManager entityManager)
            : base(entityManager)
        {
            // Initialize component lists
            this.textureCollection = textureCollection;
            this.modelCollection = modelCollection;

            // Load important components
            sprites = components[ComponentType.Sprite] as Sprite[];
            screenPosition = components[ComponentType.ScreenPosition] as ScreenPosition[];

            // Set the effect
            this.basicEffect = basicEffect;
            SetupEffect();
        }

        private void SetupEffect()
        {
            // primitive color
            basicEffect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);

            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f); // a red light
            basicEffect.DirectionalLight0.Direction = new Vector3(1, -1, 0.4f);  // coming along the x-axis
            basicEffect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0); // with green highlights
            basicEffect.DirectionalLight0.Enabled = true;

            basicEffect.DiffuseColor = Vector3.One;
            basicEffect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
            basicEffect.SpecularPower = 1.0f;
            basicEffect.Alpha = 1.0f;

            basicEffect.LightingEnabled = true;
        }

        /// <summary>
        /// Process entities
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            for (int i = 0; i < totalEntities; i++)
            {
                // Update sprite animations
                if (sprites[i] != null && sprites[i].live && sprites[i].animation != Animation.None)
                {
                    // Move time forward
                    if (sprites[i].animation == Animation.Forward || sprites[i].animation == Animation.DualForward)
                        sprites[i].frameTime += (float)frameStepTime.TotalSeconds;

                    // Move time backward
                    if (sprites[i].animation == Animation.Reverse || sprites[i].animation == Animation.DualReverse)
                        sprites[i].frameTime -= (float)frameStepTime.TotalSeconds;

                    if (sprites[i].frameTime > sprites[i].frameLength)
                    {
                        // Move to the next frame
                        sprites[i].frame++;

                        if (sprites[i].frame >= sprites[i].frameCount)
                        {
                            if (sprites[i].animation == Animation.DualForward)
                            {
                                sprites[i].animation = Animation.DualReverse;
                                sprites[i].frame--;
                            }

                            if (sprites[i].animation == Animation.Forward)
                                sprites[i].frame = 0;
                        }

                        // Rewind frame time
                        sprites[i].frameTime -= sprites[i].frameLength;
                    }

                    if (sprites[i].frameTime < 0)
                    {
                        // Move to the previous frame
                        sprites[i].frame--;

                        if (sprites[i].frame < 0)
                        {
                            if (sprites[i].animation == Animation.DualReverse)
                            {
                                sprites[i].animation = Animation.DualForward;
                                sprites[i].frame++;
                            }

                            if (sprites[i].animation == Animation.Reverse)
                                sprites[i].frame = sprites[i].frameCount - 1;
                        }

                        // Add frame time
                        sprites[i].frameTime += sprites[i].frameLength;
                    }
                }
            }

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
                    Rectangle finalArea = sprites[i].textureArea;
                    finalArea.X += sprites[i].frame * sprites[i].textureArea.Width;

                    // Draw the sprite
                    spriteBatch.Draw(textureCollection[sprites[i].spriteTexture],
                        screenPosition[i].position + screenAreaOffset, finalArea, Color.White);
                }
            }
            // Finish drawing entities
            spriteBatch.End();
            /*
            for (int i = 0; i < totalEntities; i++)
            {
                // Check valid Sprite components
                if (sprites[i] != null && sprites[i].live)
                {
                    Vector3 position = Vector3.Zero;
                    position.X = screenPosition[i].position.X / 35f;
                    position.Z = screenPosition[i].position.Y / 35f;

                    // Test draw cube
                    foreach (ModelMesh mesh in modelCollection["block1"].Meshes)
                    {
                        basicEffect.World = Matrix.CreateTranslation(position);

                        // Set the mesh once before setting the transforms
                        graphicsDevice.SetVertexBuffer(mesh.MeshParts[0].VertexBuffer);
                        graphicsDevice.Indices = mesh.MeshParts[0].IndexBuffer;

                        // Render each instance of this mesh
                        if (mesh.MeshParts[0].IndexBuffer == null)
                            continue;

                        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            graphicsDevice.DrawIndexedPrimitives(
                                PrimitiveType.TriangleList, 0, 0,
                                mesh.MeshParts[0].IndexBuffer.IndexCount, 0,
                                mesh.MeshParts[0].IndexBuffer.IndexCount / 3);
                        }
                    }
                }
            } */
            // Finish test draw
            
        }
    }
}
