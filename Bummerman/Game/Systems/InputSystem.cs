using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bummerman.Systems
{
    /// <summary>
    /// Handle input from players for processing in other systems
    /// </summary>
    class InputSystem : EntitySystem
    {
        KeyboardState previousKeyboardState;
        KeyboardState currentKeyboardState;
        GamePadState previousGamePadState;
        GamePadState currentGamePadState;

        /// Important components
        Components.InputContext[] inputContext;
        Components.PlayerInfo[] playerInfo;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public InputSystem(EntityManager entityManager)
            : base(entityManager)
        {
            // Load important components
            inputContext = components[ComponentType.InputContext] as Components.InputContext[];
            playerInfo = components[ComponentType.PlayerInfo] as Components.PlayerInfo[];
        }

        /// <summary>
        /// Process entities with Input components
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            this.totalEntities = totalEntities;

            // Should initialize a selectable PlayerIndex
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // First, reset player input action messages for each frame
            GetMessage(MessageType.InputAction1).messageID = 0;
            GetMessage(MessageType.InputState1).messageID = 0;
            GetMessage(MessageType.InputAction2).messageID = 0;
            GetMessage(MessageType.InputState2).messageID = 0;

            for (int entity = 0; entity < totalEntities; entity++)
            {
                if (inputContext[entity] != null)
                {
                    Components.InputContext context = inputContext[entity];

                    // Set old inputs to previous one
                    context.previousAction = context.currentAction;
                    context.previousState = context.currentState;

                    // Reset inputs
                    context.currentAction = 0;
                    context.currentState = 0;

                    int playerID = playerInfo[entity].playerNumber - 1;

                    foreach (KeyValuePair<Keys, InputActions> action in context.keyToActions)
                    {
                        // Store the last input action
                        if (!previousKeyboardState.IsKeyDown(action.Key) && currentKeyboardState.IsKeyDown(action.Key))
                        {
                            context.currentAction = (uint)Convert.ToInt16(action.Value);

                            //GetMessage(MessageType.InputAction1 + playerID).messageID = (uint)Convert.ToInt16(action.Value);
                            //GetMessage(MessageType.InputAction1 + playerID).receiver = (uint)Convert.ToInt16(entity);
                        }
                    }

                    foreach (KeyValuePair<Keys, InputStates> state in context.keyToStates)
                    {
                        // Store input states in messages. Input states are cumulative
                        if (currentKeyboardState.IsKeyDown(state.Key))
                        {
                            context.currentState |= (uint)1 << Convert.ToInt16(state.Value);

                            //GetMessage(MessageType.InputState1 + playerID).messageID |= (uint)1 << Convert.ToInt16(state.Value);
                            //GetMessage(MessageType.InputState1 + playerID).receiver = (uint)Convert.ToInt16(entity);
                        }
                    }
                }
            }

            // Map to previous input states for next frame
            previousKeyboardState = currentKeyboardState;
            previousGamePadState = currentGamePadState;

            return base.Process(frameStepTime, totalEntities);
        }
    }
}
