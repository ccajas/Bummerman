using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bummerman.Components;
using Meteor.ECS;

namespace Bummerman
{
    /// <summary>
    /// Renders all entities with a Sprite component.
    /// </summary>
    class SpriteRenderSystem : DrawableEntitySystem
    {
        /// Reference to texture and model assets
        Dictionary<string, Texture2D> textureCollection;
        //Dictionary<string, Model> modelCollection;

        /// Graphics resources
        SpriteBatch spriteBatch;

        /// Offset position
        Vector2 screenAreaOffset = new Vector2(144, 10);

        /// Important components
        Sprite[] sprites;
        ScreenPosition[] screenPosition;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public SpriteRenderSystem(ComponentManager componentManager,
            Dictionary<string, Texture2D> textureCollection,
            SpriteBatch spriteBatch)
            : base(componentManager)
        {
            // Initialize asset collections
            this.textureCollection = textureCollection;

            // Load important components
            sprites = components[(int)ComponentType.Sprite] as Sprite[];
            screenPosition = components[(int)ComponentType.ScreenPosition] as ScreenPosition[];

            // Set the effect
            this.spriteBatch = spriteBatch;
            //SetupEffect();
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
                    // Update time for Forward animations
                    if (sprites[i].animation == Animation.Forward || 
                        sprites[i].animation == Animation.DualForward || sprites[i].animation == Animation.NotLooped)
                        sprites[i].frameTime += (float)frameStepTime.TotalSeconds;

                    // Update time for Reverse animations
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
        public override void Draw()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.Default, RasterizerState.CullCounterClockwise);

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
        }
    }
}
