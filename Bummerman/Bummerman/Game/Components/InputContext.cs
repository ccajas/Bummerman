using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bummerman.Components
{
    class InputContext : Component
    {
        public override ComponentType type { get { return ComponentType.InputContext; } }

        public Dictionary<Keys, InputActions> keyToActions { get; private set; }
        public Dictionary<Keys, InputActions> keyToStates { get; private set; }
        public Dictionary<Buttons, InputStates> buttonToActions { get; private set; }
        public Dictionary<Buttons, InputStates> buttonToStates { get; private set; }
    }
}
