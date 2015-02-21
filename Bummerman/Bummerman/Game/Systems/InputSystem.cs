using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bummerman
{
    class InputSystem : EntitySystem
    {
        KeyboardState previousKeyboardState;
        KeyboardState currentKeyboardState;
        GamePadState previousGamePadState;
        GamePadState currentGamePadState;

        List<InputActions> actionsWorker;
        List<InputStates> statesWorker;

        // Local components
        Components.InputContext[] inputComponents;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public InputSystem(ComponentCollection collection) : base(collection)
        {
            // Initialize worker lists
            actionsWorker = new List<InputActions>();
            statesWorker = new List<InputStates>();
        }

        /// <summary>
        /// Process entities with Input components
        /// </summary>
        public override void Process(TimeSpan frameStepTime, int totalEntities)
        {
            // Should initialize a selectable PlayerIndex
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            this.totalEntities = totalEntities;

            for (int i = 0; i < totalEntities; i++)
            {
                if (inputComponents[i] != null)
                {
                    Components.InputContext inputContext = inputComponents[i];

                    foreach (KeyValuePair<Keys, InputActions> pair in inputContext.keyToActions)
                    {
                        if (!previousKeyboardState.IsKeyDown(pair.Key) && currentKeyboardState.IsKeyDown(pair.Key))
                            actionsWorker.Add(pair.Value);
                    }

                    foreach (KeyValuePair<Keys, InputStates> pair in inputContext.keyToStates)
                    {
                        if (currentKeyboardState.IsKeyDown(pair.Key))
                            statesWorker.Add(pair.Value);
                    }
                }
            }

            // Map to previous input states for next frame
            previousKeyboardState = currentKeyboardState;
            previousGamePadState = currentGamePadState;

            base.Process(frameStepTime, totalEntities);
        }
    }
}
