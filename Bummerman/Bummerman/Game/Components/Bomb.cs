using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman.Components
{
    class Bomb : Component
    {
        public override ComponentType type { get { return ComponentType.Bomb; } }

        public int ownerID = 0;
        public int bombType = 0;
        public int power = 2;
    }
}
