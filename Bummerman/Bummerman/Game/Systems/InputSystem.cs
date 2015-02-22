using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bummerman.Systems
{
    class InputSystem : EntitySystem
    {
        KeyboardState previousKeyboardState;
        KeyboardState currentKeyboardState;
        GamePadState previousGamePadState;
        GamePadState currentGamePadState;

        List<InputActions> actionsWorker;
        List<InputStates> statesWorker;

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
            this.totalEntities = totalEntities;

            // Should initialize a selectable PlayerIndex
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // First, reset player input action messages for each frame
            GetMessage(MessageType.InputAction1).messageID = 0;

            Components.InputContext[] inputContext = components.inputContext;

            for (int i = 0; i < totalEntities; i++)
            {
                if (inputContext[i] != null)
                {
                    Components.InputContext context = inputContext[i];

                    foreach (KeyValuePair<Keys, InputActions> pair in context.keyToActions)
                    {
                        if (!previousKeyboardState.IsKeyDown(pair.Key) && currentKeyboardState.IsKeyDown(pair.Key))
                            actionsWorker.Add(pair.Value);
                    }

                    foreach (KeyValuePair<Keys, InputStates> pair in context.keyToStates)
                    {
                        if (currentKeyboardState.IsKeyDown(pair.Key))
                            statesWorker.Add(pair.Value);
                    }
                }
            }

            // Store input states and actions in messages
            // Input states are cumulative. Actions are handled sequentially

            foreach (InputStates state in statesWorker)
                GetMessage(MessageType.InputState1).messageID |= (uint)1 << Convert.ToInt16(state);

            // Store the last input action
            foreach (InputStates action in actionsWorker)
                GetMessage(MessageType.InputAction1).messageID = (uint)Convert.ToInt16(action);

            statesWorker.Clear();
            actionsWorker.Clear();

            // Map to previous input states for next frame
            previousKeyboardState = currentKeyboardState;
            previousGamePadState = currentGamePadState;

            base.Process(frameStepTime, totalEntities);
        }
    }
}
