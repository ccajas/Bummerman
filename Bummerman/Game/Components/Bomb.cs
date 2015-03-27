using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meteor.ECS;

namespace Bummerman.Components
{
    class Bomb : Component
    {
        public override Int32 type { get { return (int)ComponentType.Bomb; } }

        public int ownerID = 0;
        public int bombType = 0;
        public int power = 2;
    }
}
