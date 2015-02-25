using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman.Components
{
    class PowerUp : Component
    {
        public override ComponentType type { get { return ComponentType.PowerUp; } }

        public int bombUprade = 0;
        public int powerUpgrade = 0;
        public int speedUpgrade = 0;
        public bool kickUpgrade = false;
        public bool throwUpgrade = false;
    }
}
