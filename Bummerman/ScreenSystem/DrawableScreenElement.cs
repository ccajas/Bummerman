using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman
{
    public abstract class DrawableScreenElement : ScreenElement
    {
        /// Viewport and graphics system
        protected Viewport viewport;
        protected GraphicsDevice graphicsDevice;

        /// Spritebatch reference
        protected SpriteBatch spriteBatch;

        // Background pixel texture 
        protected static Texture2D pixel;
        private static Color[] colorData = { Color.White };  

        /// <summary>
        /// Creates the ScreenElement
        /// </summary>
        public DrawableScreenElement(ScreenElement previousScreenElement, GraphicsDevice graphicsDevice) :
            base(previousScreenElement)
        {
            // Add graphics systems
            this.graphicsDevice = graphicsDevice;
            this.viewport = graphicsDevice.Viewport;

            // Create a SpriteBatch and pixel texture
            spriteBatch = new SpriteBatch(graphicsDevice);
            ColorRectangle(graphicsDevice);

            // Handle device reset
            graphicsDevice.DeviceReset += OnDeviceReset;
        }

        /// <summary>
        /// Make a solid color rectangle
        /// </summary>
        public void ColorRectangle(GraphicsDevice device)
        {
            // Make a 1x1 texture named pixel.  
            pixel = new Texture2D(device, 1, 1);

            // Set the texture data with our color information.  
            pixel.SetData<Color>(colorData);
        }

        /// <summary>
        /// Called when the graphics device is reset
        /// </summary>
        protected virtual void OnDeviceReset(Object sender, EventArgs args) { }

        /// <summary>
        /// Draw contents of the screen
        /// </summary>
        public virtual void Draw(TimeSpan frameStepTime)
        {
            foreach (DrawableScreenElement screen in children)
                screen.Draw(frameStepTime);
        }

        /// <summary>
        /// Unload and remove attached events
        /// </summary>
        public override void UnloadContent()
        {
            // Remove device reset handler
            graphicsDevice.DeviceReset -= OnDeviceReset;

            base.UnloadContent();
        }
    }
}
