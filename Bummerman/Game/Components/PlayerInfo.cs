﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meteor.ECS;

namespace Bummerman.Components
{
    class PlayerInfo : Component
    {
        public override Int32 type { get { return (int)ComponentType.PlayerInfo; } }

        public int playerNumber = 0;
        public int bombPower = 1;
        public int currentBombs = 0;
        public int maxBombs = 1;

        public int score = 0;
        public float speed = 100;
    }
}
