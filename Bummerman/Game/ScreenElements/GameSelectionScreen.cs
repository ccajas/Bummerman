using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman.ScreenElements
{
    class GameSelectionScreen : DrawableScreenElement
    {
        // Sprite textures and other assets
        SpriteFont debugFont;

        /// last frame's keyboard state
        KeyboardState previousKeyboardState;

        public GameSelectionScreen(Game game, ScreenElement previousScreenElement)
            : base(previousScreenElement, game.GraphicsDevice)
        {
            // Load assets
            debugFont = game.Content.Load<SpriteFont>("debug");

            previousKeyboardState = Keyboard.GetState();
            this.game = game;
        }

        public override ScreenElement Update(TimeSpan frameStepTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A) && !previousKeyboardState.IsKeyDown(Keys.A))
                return new GameScreen(this.game, this);

            previousKeyboardState = Keyboard.GetState();

            return base.Update(frameStepTime);
        }

        /// <summary>
        /// Draw the game selection screen
        /// </summary>
        public override void Draw(TimeSpan frameStepTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(debugFont, "Press 'A' to host a game, or 'S' to join an existing one",
                new Vector2(2, 20), Color.White);
            spriteBatch.End();

            base.Draw(frameStepTime);
        }
    }
}
