﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meteor.ECS;

namespace Bummerman.Components
{
    class PowerUp : Component
    {
        public override Int32 type { get { return (int)ComponentType.PowerUp; } }

        public int bombUpgrade = 0;
        public int powerUpgrade = 0;
        public int speedUpgrade = 0;
        public bool kickUpgrade = false;
        public bool throwUpgrade = false;
    }
}
