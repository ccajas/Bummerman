using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman.Components
{
    class PlayerInfo : Component
    {
        public override ComponentType type { get { return ComponentType.PlayerInfo; } }

        public int playerNumber = 0;
        public int bombPower = 1;
        public int currentBombs = 0;
        public int maxBombs = 1;

        public float speed = 100;
    }
}
