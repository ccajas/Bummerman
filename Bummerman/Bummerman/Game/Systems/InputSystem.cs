using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bummerman.Systems
{
    using ComponentCollection = Dictionary<ComponentType, Component[]>;

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
        List<InputStates> statesWorker;

        /// Important components
        Components.InputContext[] inputContext;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public InputSystem(EntityManager entityManager)
            : base(entityManager)
        {
            // Initialize worker lists
            actionsWorker = new Dictionary<InputActions, uint>();
            statesWorker = new List<InputStates>();

            // Load important components
            inputContext = components[ComponentType.InputContext] as Components.InputContext[];
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

            for (int entity = 0; entity < totalEntities; entity++)
            {
                if (inputContext[entity] != null)
                {
                    Components.InputContext context = inputContext[entity];

                    foreach (KeyValuePair<Keys, InputActions> pair in context.keyToActions)
                    {
                        if (!previousKeyboardState.IsKeyDown(pair.Key) && currentKeyboardState.IsKeyDown(pair.Key))
                            actionsWorker[pair.Value] = (uint)entity;
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
            foreach (KeyValuePair<InputActions, uint> action in actionsWorker)
            {
                GetMessage(MessageType.InputAction1).messageID = (uint)Convert.ToInt16(action.Key);
                GetMessage(MessageType.InputAction1).data = (uint)Convert.ToInt16(action.Value);
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
