using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Bummerman.Components;
using Meteor.ECS;

namespace Bummerman
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
        InputContext[] inputContext;
        PlayerInfo[] playerInfo;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public InputSystem(ComponentManager componentManager)
            : base(componentManager)
        {
            // Load important components
            inputContext = components[ComponentType.InputContext] as InputContext[];
            playerInfo = components[ComponentType.PlayerInfo] as PlayerInfo[];
        }

        /// <summary>
        /// Process entities with Input components
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            // TODO: Should initialize a selectable PlayerIndex
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Check all entities with an InputContext
            for (int entity = 0; entity < totalEntities; entity++)
            {
                if (inputContext[entity] != null)
                {
                    InputContext context = inputContext[entity];

                    // Set old inputs to previous one
                    context.previousAction = context.currentAction;
                    context.previousState = context.currentState;

                    // Set non-server controlled inputs
                    if (!context.updatedByServer)
                    {
                        // Reset inputs
                        context.currentAction = 0;
                        context.currentState = 0;

                        foreach (KeyValuePair<Keys, InputActions> action in context.keyToActions)
                        {
                            // Store the last input action
                            if (!previousKeyboardState.IsKeyDown(action.Key) && currentKeyboardState.IsKeyDown(action.Key))
                                context.currentAction = (uint)Convert.ToInt16(action.Value);
                        }

                        foreach (KeyValuePair<Keys, InputStates> state in context.keyToStates)
                        {
                            // Store input states. Input states are cumulative
                            if (currentKeyboardState.IsKeyDown(state.Key))
                                context.currentState |= (uint)1 << Convert.ToInt16(state.Value);
                        }
                    }
                    else
                        context.updatedByServer = false;             
                }
            }

            // Map to previous input states for next frame
            previousKeyboardState = currentKeyboardState;
            previousGamePadState = currentGamePadState;

            return base.Process(frameStepTime, totalEntities);
        }
    }
}
