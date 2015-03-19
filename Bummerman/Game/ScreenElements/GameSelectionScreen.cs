using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bummerman.ScreenElements
{
    class GameSelectionScreen : DrawableScreenElement
    {
        /// last frame's keyboard state
        KeyboardState previousKeyboardState;

        public GameSelectionScreen(Game game, ScreenElement previousScreenElement)
            : base(previousScreenElement, game.GraphicsDevice)
        {
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
            base.Draw(frameStepTime);
        }
    }
}
