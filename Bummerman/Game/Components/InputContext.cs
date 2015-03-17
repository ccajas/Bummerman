using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Meteor.ECS;

namespace Bummerman.Components
{
    class InputContext : Component
    {
        public override ComponentType type { get { return ComponentType.InputContext; } }

        public Dictionary<Keys, InputActions> keyToActions { get; private set; }
        public Dictionary<Keys, InputStates> keyToStates { get; private set; }
        public Dictionary<Buttons, InputActions> buttonToActions { get; private set; }
        public Dictionary<Buttons, InputStates> buttonToStates { get; private set; }

        /// Current inputs of this context
        public uint currentAction = 0;
        public uint currentState = 0;

        /// Previous inputs of this context
        public uint previousAction = 0;
        public uint previousState = 0;

        /// <summary>
        /// Initialize mapping lists
        /// </summary>
        public InputContext()
        {
            keyToActions = new Dictionary<Keys, InputActions>();
            keyToStates = new Dictionary<Keys, InputStates>();
            buttonToActions = new Dictionary<Buttons, InputActions>();
            buttonToStates = new Dictionary<Buttons, InputStates>();
        }

        /// <summary>
        /// Helper setup of input mapping
        /// </summary>
        public void SetInputs(
            IEnumerable<KeyValuePair<Keys, InputActions>> actions,
            IEnumerable<KeyValuePair<Keys, InputStates>> states = null)
        {
            foreach (KeyValuePair<Keys, InputActions> pair in actions)
            {
                keyToActions[pair.Key] = pair.Value;
            }

            if (states != null)
            {
                foreach (KeyValuePair<Keys, InputStates> pair in states)
                {
                    keyToStates[pair.Key] = pair.Value;
                }
            }
            // Finish input mapping
        }

        /// <summary>
        /// Compare input ID as a bitmask with a generic value
        /// </summary>
        public bool ValueFound<T>(T value)
        {
            int compareBit = (1 << Convert.ToInt32(value));

            return ((currentState & compareBit) == compareBit);
        }
    }
}
