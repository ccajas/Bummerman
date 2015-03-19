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
        public GameSelectionScreen(Game game, ScreenElement previousScreenElement)
            : base(previousScreenElement, game.GraphicsDevice)
        {

        }

        public override ScreenElement Update(TimeSpan frameStepTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                return new GameScreen(this.game, this);

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
