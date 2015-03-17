using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Meteor.ECS;

namespace Bummerman.Components
{
    class ScreenPosition : Component
    {
        public override ComponentType type { get { return ComponentType.ScreenPosition; } }

        public Vector2 position = Vector2.Zero;
        public Point offset = Point.Zero;
        public int layer = 0;
    }
}
