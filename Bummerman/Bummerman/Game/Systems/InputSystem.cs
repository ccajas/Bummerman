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

        /// Storage for received input
        Dictionary<InputActions, uint> actionsWorker;
        Dictionary<InputStates, uint> statesWorker;

        /// Important components
        Components.InputContext[] inputContext;
        Components.PlayerInfo[] playerInfo;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public InputSystem(EntityManager entityManager)
            : base(entityManager)
        {
            // Initialize worker lists
            actionsWorker = new Dictionary<InputActions, uint>();
            statesWorker = new Dictionary<InputStates, uint>();

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

            for (int entity = 0; entity < totalEntities; entity++)
            {
                if (inputContext[entity] != null)
                {
                    Components.InputContext context = inputContext[entity];
                    int playerID = playerInfo[entity].playerNumber - 1;

                    foreach (KeyValuePair<Keys, InputActions> pair in context.keyToActions)
                    {
                        if (!previousKeyboardState.IsKeyDown(pair.Key) && currentKeyboardState.IsKeyDown(pair.Key))
                            actionsWorker[pair.Value] = (uint)entity;
                    }

                    foreach (KeyValuePair<Keys, InputStates> state in context.keyToStates)
                    {
                        // Store input states in messages. Input states are cumulative
                        if (currentKeyboardState.IsKeyDown(state.Key))
                        {
                            GetMessage(MessageType.InputState1).messageID |= (uint)1 << Convert.ToInt16(state.Value);
                            GetMessage(MessageType.InputState1).receiver = (uint)Convert.ToInt16(entity);
                        }
                    }
                }
            }

            // Store the last input action
            foreach (KeyValuePair<InputActions, uint> action in actionsWorker)
            {
                GetMessage(MessageType.InputAction1).messageID = (uint)Convert.ToInt16(action.Key);
                GetMessage(MessageType.InputAction1).receiver = (uint)Convert.ToInt16(action.Value);
            }

            statesWorker.Clear();
            actionsWorker.Clear();

            // Map to previous input states for next frame
            previousKeyboardState = currentKeyboardState;
            previousGamePadState = currentGamePadState;

            return base.Process(frameStepTime, totalEntities);
        }
    }
}
