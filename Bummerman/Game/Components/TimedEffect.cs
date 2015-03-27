using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meteor.ECS;

namespace Bummerman.Components
{
    class TimedEffect : Component
    {
        public override Int32 type { get { return (int)ComponentType.TimedEffect; } }

        public float elapsed = 0;
    }
}
