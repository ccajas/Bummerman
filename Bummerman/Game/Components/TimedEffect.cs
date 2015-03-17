using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meteor.ECS;

namespace Bummerman.Components
{
    class TimedEffect : Component
    {
        public override ComponentType type { get { return ComponentType.TimedEffect; } }

        public float elapsed = 0;
    }
}
