using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meteor.ECS;

namespace Bummerman.Components
{
    enum Direction
    {
        None,
        Left,
        Right,
        Top,
        Bottom
    }

    class Spreadable : Component
    {
        public override Int32 type { get { return (int)ComponentType.Spreadable; } }

        public int range = 0;
        public Direction direction = Direction.None;
    }
}
