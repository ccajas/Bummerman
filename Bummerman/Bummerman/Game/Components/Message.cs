using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman.Components
{
    class Message : Component
    {
        public override ComponentType type { get { return ComponentType.Message; } }

        public int messageID = -1;
        public int data = 0;
    }
}
